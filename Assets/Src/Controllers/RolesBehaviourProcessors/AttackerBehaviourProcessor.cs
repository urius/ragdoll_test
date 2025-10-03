using Src.Data;
using Src.Data.BehaviourStates;
using Src.DebugComponents;
using Src.Model;
using Src.Providers;
using UnityEngine;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class AttackerBehaviourProcessor
    {
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;

        public AttackerBehaviourProcessor(
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider)
        {
            _ballPositionProvider = ballPositionProvider;
            _goalGatesProvider = goalGatesProvider;
        }

        public void Process(IFootballerUnit footballer)
        {
            switch (footballer.BehaviourState)
            {
                case BehaviourStateName.Undefined:
                    DefineState(footballer);
                    break;
                case BehaviourStateName.InterceptingBall:
                    if (CheckBallIsAhead(footballer))
                    {
                        ResetAndProcessState(footballer);
                    }
                    else
                    {
                        UpdateInterceptionState(footballer);
                    }
                    break;
                case BehaviourStateName.LeadTheBall:
                    if (CheckBallIsAhead(footballer) == false)
                    {
                        ResetAndProcessState(footballer);
                    }
                    break;
            }

            if (footballer.IsOnTargetPoint())
            {
                ProcessNextState(footballer);
            }
        }

        private void ResetAndProcessState(IFootballerUnit footballer)
        {
            footballer.ResetBehaviourState();
            Process(footballer);
        }

        private void ProcessNextState(IFootballerUnit footballerUnit)
        {
            Debug.Log("ProcessNextState " + footballerUnit.Team);
        }

        private void DefineState(IFootballerUnit footballer)
        {
            if (CheckBallIsAhead(footballer))
            {
                footballer.SetLeadTheBallState();
            }
            else
            {
                UpdateInterceptionState(footballer);
            }
        }

        private void UpdateInterceptionState(IFootballerUnit footballer)
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
            var xOffset = (ballPosition.x < 0 ? 1 : -1) * 5;
            var zOffset = teamSign * 5;
            var offset = new Vector3(xOffset, 0, zOffset);

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