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

        public AttackerBehaviourProcessor(
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider,
            IFieldBorderPositionProvider borderPositionProvider)
        {
            _ballPositionProvider = ballPositionProvider;
            _goalGatesProvider = goalGatesProvider;
            _borderPositionProvider = borderPositionProvider;
        }

        public void Process(IFootballerUnit footballer)
        {
            switch (footballer.BehaviourState)
            {
                case BehaviourStateName.Undefined:
                    DefineState(footballer);
                    break;
                case BehaviourStateName.InterceptingBall:
                    if (footballer.IsOnTargetPoint() || 
                        (_borderPositionProvider.IsCloseToShortBorder(footballer.PositionProjected) && CheckBallIsAhead(footballer)))
                    {
                        UpdateLeadTheBallState(footballer);
                    }
                    else
                    {
                        UpdateBallInterceptionState(footballer);
                    }
                    break;
                case BehaviourStateName.LeadTheBall:
                    if (CheckBallIsAhead(footballer) == false)
                    {
                        ResetAndProcessState(footballer);
                    }
                    else if (_borderPositionProvider.IsNearAnyBorder(footballer.Position)
                             && IsBallNear(footballer))
                    {
                        HitBallTo(footballer, _goalGatesProvider.GetGatesForTeam(footballer.Team.OppositeTeam()).Position);
                    }
                    else
                    {
                        UpdateLeadTheBallState(footballer);
                    }
                    break;
            }
        }

        private float DistanceToBall(IFootballerUnit footballer)
        {
            return Vector3.Distance(footballer.PositionProjected, _ballPositionProvider.PositionProjected);
        }

        private void HitBallTo(IFootballerUnit footballer, Vector3 targetPosition)
        {
            Debug.Log("HitBallTo");

            var direction = (targetPosition - footballer.PositionProjected).normalized * 50;
            direction.y = 7;
            footballer.SetHittingBallState(direction);
        }

        private bool IsBallNear(IFootballerUnit footballer)
        {
            return DistanceToBall(footballer) < 4.5;
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

        private bool CheckBallIsAhead(IFootballerUnit footballer)
        {
            var teamSign = GetTeamSign(footballer);
            var unitRelativeToBallPositionSign = (footballer.Position.z - _ballPositionProvider.Position.z) > 0 ? 1 : -1;
            var ballIsAhead = unitRelativeToBallPositionSign == teamSign;

            return ballIsAhead;
        }

        private Vector3 GetInterceptionOffset(int teamSign)
        {
            var ballPosition = _ballPositionProvider.Position;
            var xOffset = (ballPosition.x < 0 ? 1 : -1) * 3;
            var zOffset = teamSign * 3;
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