using Src.Model;
using UnityEngine;

namespace Src.Components
{
    public class BallModelFacade : MonoBehaviour, IBallModel
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
        public Vector3 LinearVelocity => _rigidbody.linearVelocity;

        public void AddLinearVelocity(Vector3 deltaVelocity)
        {
            _rigidbody.linearVelocity += deltaVelocity;
        }
        
        public void SetLinearVelocity(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
        }

        public void SetPosition(Vector3 position)
        {
            _rigidbody.position = position;
        }
    }
}