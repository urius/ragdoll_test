using System.Linq;
using Src.Extensions;
using Src.Model;
using Src.Providers;
using UnityEngine;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class GoalkeeperBehaviourProcessor : FootballerBehaviourProcessorBase
    {
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;

        public GoalkeeperBehaviourProcessor(
            IGameUnitsProvider unitsProvider,
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider) 
            : base(unitsProvider, ballPositionProvider, goalGatesProvider)
        {
            _ballPositionProvider = ballPositionProvider;
            _goalGatesProvider = goalGatesProvider;
        }

        public override void Process(IFootballerUnit footballer)
        {
            var gates = _goalGatesProvider.GetGatesForTeam(footballer.Team);
            
            if (IsBallNear(footballer))
            {
                footballer.SetHittingBallState(gates.Forward, 50, 10);
                return;
            }

            var ballToGatesDistance =
                Vector3.Distance(_goalGatesProvider.GetGatesForTeam(footballer.Team).Position, _ballPositionProvider.Position);
            
            var ballPositionWithPrediction = _ballPositionProvider.PositionProjected + 0.5f * _ballPositionProvider.LinearVelocity.Projected();

            if (ballToGatesDistance < 70)
            {
                var distanceToBall = (_ballPositionProvider.PositionProjected - footballer.PositionProjected).magnitude;
                var ballToClosestEnemyDistance =
                    (GetClosestEnemy(footballer).PositionProjected - _ballPositionProvider.PositionProjected).magnitude;
            
                if (ballToClosestEnemyDistance > distanceToBall)
                {
                    footballer.SetMoveToTargetPointState(ballPositionWithPrediction);
                    return;
                }
            }

            var gateBounds = gates.BoundPositions;
            var gateMaxXBounds = gateBounds.Max(v => v.x);
            var gateMinXBounds = gateBounds.Min(v => v.x);
            var targetGoalKeeperXPosition = Mathf.Clamp(ballPositionWithPrediction.x, gateMinXBounds, gateMaxXBounds);

            var targetPos = new Vector3(targetGoalKeeperXPosition, 0, gateBounds[0].z);
            if (Vector3.Distance(footballer.PositionProjected, targetPos) > 1)
            {
                footballer.SetMoveToTargetPointState(targetPos);
            }
            else
            {
                footballer.SetTargetDirection(gates.Forward);
            }
        }

        private bool IsBallNear(IFootballerUnit footballer)
        {
            return Vector3.Distance(footballer.PositionProjected, _ballPositionProvider.PositionProjected) < 4;
        }
    }
}