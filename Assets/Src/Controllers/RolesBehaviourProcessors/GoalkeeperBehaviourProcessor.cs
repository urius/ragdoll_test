using System.Linq;
using Src.Model;
using Src.Providers;
using UnityEngine;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class GoalkeeperBehaviourProcessor
    {
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;

        public GoalkeeperBehaviourProcessor(
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider)
        {
            _ballPositionProvider = ballPositionProvider;
            _goalGatesProvider = goalGatesProvider;
        }

        public void Process(IFootballerUnit footballer)
        {
            var ballPosition = _ballPositionProvider.Position;
            
            var gates = _goalGatesProvider.GetGatesForTeam(footballer.Team);
            var gateBounds = gates.BoundPositions;
            var gateMaxXBounds = gateBounds.Max(v => v.x);
            var gateMinXBounds = gateBounds.Min(v => v.x);
            var targetGoalKeeperXPosition = Mathf.Clamp(ballPosition.x, gateMinXBounds, gateMaxXBounds);

            var targetPos = new Vector3(targetGoalKeeperXPosition, 0, gateBounds[0].z);
            footballer.SetMovingToTargetPointState(targetPos);
            if (footballer.IsOnTargetPoint())
            {
                footballer.SetTargetDirection(gates.Forward);
            }
        }
    }
}