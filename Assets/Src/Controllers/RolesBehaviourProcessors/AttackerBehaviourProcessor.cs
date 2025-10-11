using Src.Data;
using Src.Data.BehaviourStates;
using Src.DebugComponents;
using Src.Extensions;
using Src.Model;
using Src.Providers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class AttackerBehaviourProcessor : IRoleBehaviourProcessor
    {
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;
        private readonly IFieldBorderPositionProvider _borderPositionProvider;
        private readonly IGameUnitsProvider _unitsProvider;

        public AttackerBehaviourProcessor(
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider,
            IFieldBorderPositionProvider borderPositionProvider,
            IGameUnitsProvider unitsProvider)
        {
            _ballPositionProvider = ballPositionProvider;
            _goalGatesProvider = goalGatesProvider;
            _borderPositionProvider = borderPositionProvider;
            _unitsProvider = unitsProvider;
        }

        public void Process(IFootballerUnit footballer)
        {
            switch (footballer.BehaviourState)
            {
                case BehaviourStateName.Undefined:
                    DefineState(footballer);
                    break;
                case BehaviourStateName.InterceptingBall:
                    ProcessCorrectBallWhileIntercepting(footballer);

                    UpdateBallInterceptionState(footballer);

                    if (footballer.TargetPointWasReached
                        || CheckBallIsAhead(footballer,
                            Mathf.Max(3, GetTeamSign(footballer) * _ballPositionProvider.LinearVelocity.z))
                        || (_borderPositionProvider.IsCloseToShortBorder(footballer.PositionProjected) &&
                            CheckBallIsAhead(footballer)))
                    {
                        //Debug.Log("<color=green>Set Lead the ball</color>");
                        UpdateLeadTheBallState(footballer);
                    }

                    break;
                case BehaviourStateName.LeadTheBall:
                    ProcessLeadTheBallState(footballer);
                    break;
            }
        }

        private void ProcessLeadTheBallState(IFootballerUnit footballer)
        {
            if (CheckBallIsAhead(footballer) == false)
            {
                ResetAndProcessState(footballer);
                return;
            }

            var isBallNear = IsBallNear(footballer);

            if (isBallNear)
            {
                if (ProcessPassIfOpponentIsAhead(footballer)) return;

                if (Random.value < 0.2f)
                {
                    if (ProcessTryPassToTeammateAhead(footballer)) return;
                }

                if (ProcessHitGatesByDistanceLogic(footballer)) return;

                if (_borderPositionProvider.IsNearAnyBorder(footballer.Position))
                {
                    HitGates(footballer);

                    return;
                }
            }

            if (_ballPositionProvider.Position.z.Sign() != GetTeamSign(footballer))
            {
                if (_ballPositionProvider.LinearVelocity.x.Sign() == _ballPositionProvider.Position.x.Sign()
                    && Mathf.Abs(_ballPositionProvider.PositionProjected.x) > 70)
                {
                    footballer.RequestCorrectBallSpeed(new Vector3(-1.5f * _ballPositionProvider.LinearVelocity.x, 0, 0));
                }
            }
        }

        private bool ProcessPassIfOpponentIsAhead(IFootballerUnit footballer)
        {
            var closestEnemy = GetClosestEnemy(footballer);
            if (closestEnemy != null)
            {
                var distance = Vector3.Distance(closestEnemy.PositionProjected, footballer.PositionProjected);
                if (distance < 15 && CheckPositionIsAhead(footballer, closestEnemy.Position))
                {
                    Debug.Log("Try Pass (opponent near)");
                    
                    return ProcessTryPassToTeammateAhead(footballer, -50);
                }
            }

            return false;
        }

        private bool ProcessTryPassToTeammateAhead(IFootballerUnit footballer, float relativeOffset = 0)
        {
            var closestTeammateAhead = GetClosestTeammateAhead(footballer, relativeOffset);
            if (closestTeammateAhead != null)
            {
                HitBallTo(footballer, closestTeammateAhead.PositionProjected);

                return true;
            }

            return false;
        }

        private bool ProcessHitGatesByDistanceLogic(IFootballerUnit footballer)
        {
            var zDistanceToEnemyGates = GetZDistanceToEnemyGates(footballer);

            switch (zDistanceToEnemyGates)
            {
                case < 50:
                    if (Random.value < 0.8f)
                    {
                        HitGates(footballer);
                        return true;
                    }

                    break;
                case < 100:
                    if (Random.value < 0.5f)
                    {
                        HitGates(footballer);
                        return true;
                    }

                    break;
                case < 150:
                    if (Random.value < 0.2f)
                    {
                        HitGates(footballer);
                        return true;
                    }

                    break;
            }

            return false;
        }

        private void HitGates(IFootballerUnit footballer)
        {
            HitBallTo(footballer, _goalGatesProvider.GetGatesForTeam(footballer.Team.OppositeTeam()).Position);
        }

        private float GetZDistanceToEnemyGates(IFootballerUnit footballer)
        {
            return Mathf.Abs(
                footballer.Position.z - _goalGatesProvider.GetGatesForTeam(footballer.Team.OppositeTeam()).Position.z);
        }

        private void PassBallToClosestTeammate(IFootballerUnit footballer)
        {
            var closestTeammate = GetClosestTeammate(footballer);
            if (closestTeammate != null)
            {
                HitBallTo(footballer, closestTeammate.PositionProjected);
            }
        }

        private IFootballerUnit GetClosestEnemy(IFootballerUnit footballer)
        {
            return GetClosestUnit(footballer, footballer.Team.OppositeTeam());
        }

        private IFootballerUnit GetClosestTeammate(IFootballerUnit footballer)
        {
            return GetClosestUnit(footballer, footballer.Team);
        }

        private IFootballerUnit GetClosestTeammateAhead(IFootballerUnit targetUnit, float relativeOffset = 0)
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

        private IFootballerUnit GetClosestUnit(IFootballerUnit targetUnit, TeamKey team)
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

        private float DistanceToBall(IFootballerUnit footballer)
        {
            return Vector3.Distance(footballer.PositionProjected, _ballPositionProvider.PositionProjected);
        }

        private void HitBallTo(IFootballerUnit footballer, Vector3 targetPosition, float strengthHorizontal = 50, float strengthVertical = 7)
        {
            Debug.Log("HitBallTo");

            var direction = targetPosition - footballer.PositionProjected;
            footballer.SetHittingBallState(direction, strengthHorizontal, strengthVertical);
        }

        private bool IsBallNear(IFootballerUnit footballer)
        {
            return DistanceToBall(footballer) < 4;
        }

        private void ResetAndProcessState(IFootballerUnit footballer)
        {
            footballer.ResetBehaviourState();
            Process(footballer);
        }

        private void DefineState(IFootballerUnit footballer)
        {
            if (CheckBallIsAhead(footballer))
            {
                UpdateLeadTheBallState(footballer);
            }
            else
            {
                UpdateBallInterceptionState(footballer);
            }
        }

        private void UpdateLeadTheBallState(IFootballerUnit footballer)
        {
            footballer.SetLeadTheBallState();
        }

        private void UpdateBallInterceptionState(IFootballerUnit footballer)
        {
            var offset = GetInterceptionOffset(footballer);
            footballer.SetInterceptBallState(offset);
        }

        private void ProcessCorrectBallWhileIntercepting(IFootballerUnit footballer)
        {
            var ballCorrectionVector = _ballPositionProvider.LinearVelocity.Projected().normalized * 25;
            var leftCorrectionVector = Quaternion.Euler(0, -90, 0) * ballCorrectionVector;
            var rightCorrectionVector = Quaternion.Euler(0, 90, 0) * ballCorrectionVector;

            var enemyGatesPosition = _goalGatesProvider.GetGatesForTeam(footballer.Team.OppositeTeam()).Position;
            var ballPosition = _ballPositionProvider.PositionProjected;

            footballer.RequestCorrectBallSpeed(
                (ballPosition + leftCorrectionVector - enemyGatesPosition).sqrMagnitude <
                (ballPosition + rightCorrectionVector - enemyGatesPosition).sqrMagnitude
                    ? leftCorrectionVector
                    : rightCorrectionVector);
        }

        private bool CheckBallIsAhead(IFootballerUnit footballer, float relativeOffset = 0)
        {
            return CheckPositionIsAhead(footballer, _ballPositionProvider.Position, relativeOffset);
        }
        
        private bool CheckPositionIsAhead(IFootballerUnit footballer, Vector3 targetPosition, float relativeOffset = 0)
        {
            var teamSign = GetTeamSign(footballer);
            var unitRelativeToTargetPositionSign = (footballer.Position.z - (targetPosition.z + teamSign * relativeOffset)) > 0 ? 1 : -1;
            var positionIsAhead = unitRelativeToTargetPositionSign == teamSign;

            return positionIsAhead;
        }

        private Vector3 GetInterceptionOffset(IFootballerUnit footballer)
        {
            return _ballPositionProvider.LinearVelocity * 0.6f;
            
            var teamSign = GetTeamSign(footballer);
            var ballPosition = _ballPositionProvider.Position.Projected();
            var ballLinearVelocity = _ballPositionProvider.LinearVelocity;
            
            if (ballLinearVelocity.z.Sign() == (footballer.Position.z - ballPosition.z).Sign())
            {
                return ballLinearVelocity * 0.5f;
            }
            
            var xOffset = (-ballPosition.x.Sign()) * 5f;
            var zOffset = teamSign * 5f;
            if (ballLinearVelocity.z.Sign() == teamSign)
            {
                zOffset += ballLinearVelocity.z * 0.5f;
            }
            
            var offset = new Vector3(xOffset, 0, zOffset);

            DrawGizmosComponent.RequestDraw("Offset1", GizmoType.Sphere, ballPosition + offset);

            return offset;
        }

        private int GetTeamSign(IFootballerUnit footballer)
        {
            return _goalGatesProvider.GetGatesForTeam(footballer.Team).Position.z.Sign();
        }
    }
}