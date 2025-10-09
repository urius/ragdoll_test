using Src.Data;
using Src.Data.BehaviourStates;
using Src.DebugComponents;
using Src.Extensions;
using Src.Model;
using Src.Providers;
using UnityEngine;

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
                    {
                        UpdateBallInterceptionState(footballer);

                        if (footballer.IsOnTargetPoint() ||
                            CheckBallIsAhead(footballer, 3) ||
                            (_borderPositionProvider.IsCloseToShortBorder(footballer.PositionProjected)
                             && CheckBallIsAhead(footballer)))
                        {
                            Debug.Log("<color=green>Set Lead the ball</color>");
                            UpdateLeadTheBallState(footballer);
                        }
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

        private void HitBallTo(IFootballerUnit footballer, Vector3 targetPosition)
        {
            Debug.Log("HitBallTo");

            var direction = targetPosition - footballer.PositionProjected;
            footballer.SetHittingBallState(direction, 50, 7);
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
            var teamSign = GetTeamSign(footballer);
            var offset = GetInterceptionOffset(teamSign);
            footballer.SetInterceptBallState(offset);

            if (footballer.Team == TeamKey.Blue)
            {
                DrawGizmosComponent.RequestDraw("UpdateInterceptionState", GizmoType.Sphere,
                    _ballPositionProvider.PositionProjected + offset);
            }
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

        private Vector3 GetInterceptionOffset(int teamSign)
        {
            var ballPosition = _ballPositionProvider.Position;
            var xOffset = (ballPosition.x < 0 ? 1 : -1) * 5;
            var zOffset = teamSign * 5;
            var offset = new Vector3(xOffset, 0, zOffset);
            var linearVelocityOffset = _ballPositionProvider.LinearVelocity * 0.2f;
            offset += linearVelocityOffset;

            return offset;
        }

        private int GetTeamSign(IFootballerUnit footballer)
        {
            var gatesPosition = _goalGatesProvider.GetGatesForTeam(footballer.Team).Position;
            var teamSign = gatesPosition.z > 0 ? 1 : -1;
            
            return teamSign;
        }
    }
}