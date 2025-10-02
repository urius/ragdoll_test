using Src.Providers;
using UnityEngine;

namespace Src.Components
{
    public class BallFacade : MonoBehaviour, IBallPositionProvider
    {
        [SerializeField] private Rigidbody _rigidbody;
        
        private void OnTriggerEnter(Collider other)
        {
            var goalColliderComponent = other.transform.GetComponent<GoalColliderComponent>();
            
            if (goalColliderComponent)
            {
                Debug.Log("!!!!!GOAAAAAAL!!!!!");
            }
        }

        public Vector3 Position => transform.position;
        public Vector3 PositionProjected
        {
            get
            {
                var result = Position;
                result.y = 0;
                return result;
            }
        }

        public void SetLinearVelocity(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
        }
    }
}