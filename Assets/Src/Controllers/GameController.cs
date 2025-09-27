using Src.Data;
using Src.Providers;
using VContainer.Unity;

namespace Src.Controllers
{
    public class GameController : IStartable
    {
        private readonly IGoalGatesProvider _goalGatesProvider;

        public GameController(IGoalGatesProvider goalGatesProvider)
        {
            _goalGatesProvider = goalGatesProvider;
        }

        public void Start()
        {
            SetupGoalGates();
        }

        private void SetupGoalGates()
        {
            foreach (var goalGate in _goalGatesProvider.GoalGates)
            {
                var team = goalGate.Position.z > 0 ? TeamKey.Red : TeamKey.Blue;
                goalGate.SetupTeam(team);
            }
        }
    }
}