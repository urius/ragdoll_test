using Src.Data;
using UnityEngine;

namespace Src.Components
{
    [RequireComponent(typeof(Collider))]
    public class GoalColliderComponent : MonoBehaviour
    {
        private GoalGatesFacade _goalGatesFacade;

        public TeamKey Team => _goalGatesFacade.Team;
        
        public void SetGoalGatesRef(GoalGatesFacade goalGatesFacade)
        {
            _goalGatesFacade = goalGatesFacade;
        }
    }
}