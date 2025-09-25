using UnityEngine;

namespace Src.Components
{
    public class LookToBehaviour : MonoBehaviour
    {
        private ConfigurableJoint _joint;

        private void Awake()
        {
            _joint = GetComponent<ConfigurableJoint>();
        }

        public void SetTargetLookFromPosition(Vector3 targetPosition)
        {
            var lookVector = transform.position - targetPosition;
            lookVector.y = 0;
            var lookRotation = Quaternion.LookRotation(lookVector);
            
            _joint.targetRotation = Quaternion.Inverse(lookRotation);
        }
    }
}