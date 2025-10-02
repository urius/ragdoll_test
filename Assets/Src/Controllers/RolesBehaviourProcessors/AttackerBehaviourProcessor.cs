using Src.Model;
using Src.Providers;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class AttackerBehaviourProcessor
    {
        private readonly IBallPositionProvider _ballPositionProvider;

        public AttackerBehaviourProcessor(IBallPositionProvider ballPositionProvider)
        {
            _ballPositionProvider = ballPositionProvider;
        }

        public void Process(IFootballerUnit footballer)
        {
            var ballPositionProjected = _ballPositionProvider.PositionProjected;
            
            footballer.SetMovingToTargetPointState(ballPositionProjected);
        }
    }
}