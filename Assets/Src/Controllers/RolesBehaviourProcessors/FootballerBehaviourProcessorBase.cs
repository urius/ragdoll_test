using Src.Data;
using Src.Extensions;
using Src.Model;
using Src.Providers;
using UnityEngine;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public abstract class FootballerBehaviourProcessorBase : IRoleBehaviourProcessor
    {
        private readonly IGameUnitsProvider _unitsProvider;
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;

        public FootballerBehaviourProcessorBase(
            IGameUnitsProvider unitsProvider,
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider)
        {
            _unitsProvider = unitsProvider;
            _ballPositionProvider = ballPositionProvider;
            _goalGatesProvider = goalGatesProvider;
        }

        public abstract void Process(IFootballerUnit footballer);
        

        protected bool CheckBallIsAhead(IFootballerUnit footballer, float relativeOffset = 0)
        {
            return CheckPositionIsAhead(footballer, _ballPositionProvider.Position, relativeOffset);
        }
        
        protected bool CheckPositionIsAhead(IFootballerUnit footballer, Vector3 targetPosition, float relativeOffset = 0)
        {
            var teamSign = GetTeamSign(footballer);
            var unitRelativeToTargetPositionSign = (footballer.Position.z - (targetPosition.z + teamSign * relativeOffset)) > 0 ? 1 : -1;
            var positionIsAhead = unitRelativeToTargetPositionSign == teamSign;

            return positionIsAhead;
        }

        protected bool BallIsOnTeamSide(TeamKey teamKey)
        {
            return _ballPositionProvider.Position.z.Sign() == GetTeamSign(teamKey);
        }

        protected int GetTeamSign(IFootballerUnit footballer)
        {
            return GetTeamSign(footballer.Team);
        }

        protected int GetTeamSign(TeamKey teamKey)
        {
            return _goalGatesProvider.GetGatesForTeam(teamKey).Position.z.Sign();
        }

        protected IFootballerUnit GetClosestEnemy(IFootballerUnit footballer)
        {
            return GetClosestUnit(footballer, footballer.Team.OppositeTeam());
        }

        protected IFootballerUnit GetClosestTeammate(IFootballerUnit footballer)
        {
            return GetClosestUnit(footballer, footballer.Team);
        }
        

        protected IFootballerUnit GetClosestUnit(IFootballerUnit targetUnit, TeamKey team)
        {
            IFootballerUnit result = null;
            
            foreach (var unit in _unitsProvider.Footballers)
            {
                if (unit.Team == team && unit != targetUnit)
                {
                    if (result == null 
                        || (unit.PositionProjected - targetUnit.PositionProjected).sqrMagnitude < (result.PositionProjected - targetUnit.PositionProjected).sqrMagnitude)
                    {
                        result = unit;
                    }
                }
            }

            return result;
        }

        protected bool TryPassToTeammateAhead(IFootballerUnit footballer, float relativeOffset = 0)
        {
            var closestTeammateAhead = GetClosestTeammateAhead(footballer, relativeOffset);
            if (closestTeammateAhead != null)
            {
                Debug.Log("<color=yellow>Pass to teammate</color>");
                HitBallTo(footballer, closestTeammateAhead.PositionProjected);

                return true;
            }

            return false;
        }

        protected void HitBallTo(IFootballerUnit footballer, Vector3 targetPosition, float strengthHorizontal = 50, float strengthVertical = 7)
        {
            Debug.Log("HitBallTo");

            var direction = targetPosition - footballer.PositionProjected;
            footballer.SetHittingBallState(direction, strengthHorizontal, strengthVertical);
        }

        protected void HitGates(IFootballerUnit footballer)
        {
            HitBallTo(footballer, _goalGatesProvider.GetGatesForTeam(footballer.Team.OppositeTeam()).Position);
        }

        protected IFootballerUnit GetClosestTeammateAhead(IFootballerUnit targetUnit, float relativeOffset = 0)
        {
            IFootballerUnit result = null;
            
            foreach (var unit in _unitsProvider.Footballers)
            {
                if (unit.Team == targetUnit.Team 
                    && unit != targetUnit 
                    && CheckPositionIsAhead(targetUnit, unit.PositionProjected, relativeOffset))
                {
                    if (result == null 
                        || (unit.PositionProjected - targetUnit.PositionProjected).sqrMagnitude < (result.PositionProjected - targetUnit.PositionProjected).sqrMagnitude)
                    {
                        result = unit;
                    }
                }
            }

            return result;
        }
    }
}