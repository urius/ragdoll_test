using Src.Model;
using Src.Providers;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class UndefinedRoleBehaviourProcessor : FootballerBehaviourProcessorBase
    {
        public UndefinedRoleBehaviourProcessor(
            IGameUnitsProvider unitsProvider,
            IBallPositionProvider ballPositionProvider,
            IGoalGatesProvider goalGatesProvider)
            : base(unitsProvider, ballPositionProvider, goalGatesProvider)
        {
        }

        public override void Process(IFootballerUnit _)
        {
        }
    }
}