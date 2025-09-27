using Src.Data;
using Src.Model;
using UnityEngine;

namespace Src.Components
{
    public class GoalGatesFacade : MonoBehaviour, IGoalGates
    {
        [SerializeField] private GoalColliderComponent _goalCollider;
        
        private TeamKey _team;
        private AllMeshRenderers _allMeshRenderers;

        public TeamKey Team => _team;
        public Vector3 Position => transform.position;

        private void Awake()
        {
            _allMeshRenderers = GetComponent<AllMeshRenderers>();
            _goalCollider.SetGoalGatesRef(this);
        }

        public void SetupTeam(TeamKey team)
        {
            _team = team;
        }
    }
}