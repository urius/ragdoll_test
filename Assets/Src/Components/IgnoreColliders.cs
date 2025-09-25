using UnityEngine;

namespace Src.Components
{
    public class IgnoreColliders : MonoBehaviour
    {
        [SerializeField] private Collider[] _colliders;

        private void Awake()
        {
            foreach (var colliderA in _colliders)
            {
                foreach (var colliderB in _colliders)
                {
                    Physics.IgnoreCollision(colliderA, colliderB);
                }
            }
        }
    }
}