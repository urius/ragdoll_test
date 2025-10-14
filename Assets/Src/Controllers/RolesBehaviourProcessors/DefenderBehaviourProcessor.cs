using System.Collections.Generic;
using Src.Data;
using Src.Extensions;
using Src.Model;
using Src.Providers;
using UnityEngine;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class DefenderBehaviourProcessor : FootballerBehaviourProcessorBase
    {
        private readonly IGameUnitsProvider _unitsProvider;
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;
        private readonly IFieldBoundsPositionProvider _boundsPositionProvider;
        private readonly List<IFootballerUnit> _redTeamDefendersList = new(5);
        private readonly List<IFootballerUnit> _blueTeamDefendersList = new(5);
        
        public DefenderBehaviourProcessor(
            IGameUnitsProvider unitsProvider,
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider,
            IFieldBoundsPositionProvider boundsPositionProvider)
            : base(unitsProvider, ballPositionProvider, goalGatesProvider)
        {
            _unitsProvider = unitsProvider;
            _ballPositionProvider = ballPositionProvider;
            _goalGatesProvider = goalGatesProvider;
            _boundsPositionProvider = boundsPositionProvider;
        }

        public override void Process(IFootballerUnit footballer)
        {
            var defenders = GetDefendersListByTeam(footballer.Team);
            UpdateDefendersList(defenders, footballer);

            var defendAreaBounds = GetDefendArea(footballer);

            var ballPositionPredicted = _ballPositionProvider.PositionProjected + _ballPositionProvider.LinearVelocity * 0.7f;

            if (defendAreaBounds.IsPointInsideBounds(ballPositionPredicted) || defendAreaBounds.IsPointInsideBounds(_ballPositionProvider.PositionProjected))
            {
                var teammateAttacker = GetTeammateAttacker(footballer);
                if (Vector3.Distance(footballer.PositionProjected, teammateAttacker.PositionProjected) <= 5)
                {
                    footballer.SetStandingState();
                    return;
                }
                
                var unitToBallVector = _ballPositionProvider.PositionProjected - footballer.PositionProjected;

                var unitToBallDistance = unitToBallVector.magnitude;
                var offset = Vector3.zero;
                if (unitToBallDistance > 5)
                {
                    offset = _ballPositionProvider.LinearVelocity.Projected().normalized * unitToBallDistance * 0.5f;
                }

                footballer.SetLeadTheBallState(offset);

                if (IsBallNear(footballer))
                {
                    if (false == TryPassToTeammateAhead(footballer))
                    {
                        HitGates(footballer);
                    }
                }
                else
                {
                    ProcessCorrectBallDirection(footballer);
                }
            }
            else if(BallIsOnTeamSide(footballer.Team))
            {
                var targetCoords = _boundsPositionProvider
                    .ClampByBorder(new Vector3(footballer.PositionProjected.x, 0, ballPositionPredicted.z));
                footballer.SetMoveToTargetPointState(targetCoords);
            }
            else
            {
                footballer.SetMoveToTargetPointState(defendAreaBounds.Center);
            }
        }

        private IFootballerUnit GetTeammateAttacker(IFootballerUnit footballer)
        {
            foreach (var unit in _unitsProvider.Footballers)
            {
                if (unit.Role == FootballerRole.Attacker && unit.Team == footballer.Team)
                {
                    return unit;
                }
            }

            return null;
        }

        private DefenderBounds GetDefendArea(IFootballerUnit footballer)
        {
            var defenders = GetDefendersListByTeam(footballer.Team);

            var teamSign = GetTeamSign(footballer.Team);
            var defendersCountOnVerticalHalf = Mathf.Max(1, defenders.Count / 2);
            var defenderIndex = defenders.IndexOf(footballer);
            var xPositionSign = defenderIndex <= (defendersCountOnVerticalHalf - 1) ? -1 : 1;
            var xNormalizedCoord = xPositionSign * 0.25f;
            var defenderHalfSideIndex = defenderIndex % defendersCountOnVerticalHalf;

            var zNormalizedCoord = teamSign * 0.5f * (defenderHalfSideIndex + 1f) / (defendersCountOnVerticalHalf + 1f);
            var center = _boundsPositionProvider
                .NormalizedBoundsToWorldBounds(new Vector3(xNormalizedCoord, 0, zNormalizedCoord));
            var size = _boundsPositionProvider
                .NormalizedBoundsToWorldBounds(new Vector3(0.5f, 0, 0.5f * 1f / defendersCountOnVerticalHalf));

            return new DefenderBounds(center, size);
        }

        private void UpdateDefendersList(IList<IFootballerUnit> defendersList, IFootballerUnit footballer)
        {
            var shouldInsert = true;
            
            for (var i = 0; i < defendersList.Count; i++)
            {
                var tempFootballer = defendersList[i];
                if (tempFootballer == null) continue;
                
                if (tempFootballer == footballer)
                {
                    shouldInsert = false;
                }
                else if (tempFootballer.Role != FootballerRole.Defender)
                {
                    defendersList[i] = null;
                }
            }

            if (shouldInsert)
            {
                for (var i = 0; i < defendersList.Count; i++)
                {
                    if (defendersList[i] == null)
                    {
                        defendersList[i] = footballer;
                        
                        return;
                    }
                }

                defendersList.Add(footballer);
            }
        }

        private bool IsBallNear(IFootballerUnit footballer)
        {
            return Vector3.Distance(footballer.PositionProjected, _ballPositionProvider.PositionProjected) < 4;
        }

        private IList<IFootballerUnit> GetDefendersListByTeam(TeamKey footballerTeam)
        {
            if (footballerTeam == TeamKey.Red) return _redTeamDefendersList;
            if (footballerTeam == TeamKey.Blue) return _blueTeamDefendersList;

            return null;
        }

        private void ProcessCorrectBallDirection(IFootballerUnit footballer)
        {
            var enemyGatesPosition = _goalGatesProvider.GetGatesForTeam(footballer.Team.OppositeTeam()).Position;
            var ballPosition = _ballPositionProvider.PositionProjected;

            if (Vector3.Dot(enemyGatesPosition - footballer.PositionProjected, ballPosition - footballer.PositionProjected) > 0)
            {
                footballer.RequestCorrectBallSpeed((enemyGatesPosition - ballPosition).normalized * 40 * (Random.value * 0.5f + 0.5f));
            }
            else
            {
                var ballCorrectionVector = _ballPositionProvider.LinearVelocity.Projected().normalized * 30;
                var leftCorrectionVector = Quaternion.Euler(0, -90, 0) * ballCorrectionVector;
                var rightCorrectionVector = Quaternion.Euler(0, 90, 0) * ballCorrectionVector;

                footballer.RequestCorrectBallSpeed(
                    (ballPosition + leftCorrectionVector - enemyGatesPosition).sqrMagnitude <
                    (ballPosition + rightCorrectionVector - enemyGatesPosition).sqrMagnitude
                        ? leftCorrectionVector
                        : rightCorrectionVector);
            }
        }
        
        private struct DefenderBounds
        {
            public readonly Vector3 Center;
            public readonly Vector3 Size;
            
            private readonly float _xBoundMax;
            private readonly float _xBoundMin;
            private readonly float _zBoundMax;
            private readonly float _zBoundMin;

            public DefenderBounds(Vector3 center, Vector3 size)
            {
                Center = center;
                Size = size;
                _xBoundMax = Center.x + size.x / 2;
                _xBoundMin = Center.x - size.x / 2;
                _zBoundMax = Center.z + size.z / 2;
                _zBoundMin = Center.z - size.z / 2;
            }

            public Vector3 Clamp(Vector3 coords)
            {
                var result = coords;
                
                if (coords.x > _xBoundMax) result.x = _xBoundMax;
                if (coords.x < _xBoundMin) result.x = _xBoundMin;
                if (coords.z > _zBoundMax) result.z = _zBoundMax;
                if (coords.z < _zBoundMin) result.z = _zBoundMin;

                return result;
            }

            public bool IsPointInsideBounds(Vector3 point)
            {
                return point.x <= _xBoundMax && point.x >= _xBoundMin && point.z <= _zBoundMax && point.z >= _zBoundMin;
            }
        }
    }
}