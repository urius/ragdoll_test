using UnityEngine;

namespace Src.Components
{
    public class BallFacade : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var goalColliderComponent = other.transform.GetComponent<GoalColliderComponent>();
            
            if (goalColliderComponent)
            {
                Debug.Log("!!!!!GOAAAAAAL!!!!!");
            }
        }
    }
}