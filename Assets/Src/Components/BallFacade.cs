using Src.Providers;
using UnityEngine;

namespace Src.Components
{
    public class BallFacade : MonoBehaviour, IBallPositionProvider
    {
        private void OnTriggerEnter(Collider other)
        {
            var goalColliderComponent = other.transform.GetComponent<GoalColliderComponent>();
            
            if (goalColliderComponent)
            {
                Debug.Log("!!!!!GOAAAAAAL!!!!!");
            }
        }

        public Vector3 Position => transform.position;
    }
}