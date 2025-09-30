using System.Linq;
using Src.Data;
using Src.Model;
using UnityEngine;

namespace Src.Components
{
    public class GoalGatesFacade : MonoBehaviour, IGoalGates
    {
        [SerializeField] private GoalColliderComponent _goalCollider;
        [SerializeField] private Transform[] _boundTransforms;

        private Vector3[] _boundPositions;
        
        private TeamKey _team;
        private AllMeshRenderers _allMeshRenderers;

        public TeamKey Team => _team;
        public Vector3 Position => transform.position;
        public Vector3[] BoundPositions => _boundPositions;
        public Vector3 Forward => transform.forward;

        private void Awake()
        {
            _allMeshRenderers = GetComponent<AllMeshRenderers>();
            _goalCollider.SetGoalGatesRef(this);
            
            _boundPositions = _boundTransforms.Select(t => t.position).ToArray();
        }

        public void SetupTeam(TeamKey team)
        {
            _team = team;
        }
    }
}