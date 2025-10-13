using Src.Model;
using Src.Providers;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class DefenderBehaviourProcessor : FootballerBehaviourProcessorBase
    {
        public DefenderBehaviourProcessor(
            IGameUnitsProvider unitsProvider,
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider)
            : base(unitsProvider, ballPositionProvider, goalGatesProvider)
        {
        }

        public override void Process(IFootballerUnit footballer)
        {
            
        }
    }
}