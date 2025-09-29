using UnityEngine;

namespace Src.Components
{
    public class LookToBehaviour : MonoBehaviour
    {
        private ConfigurableJoint _joint;
        private Vector3 _targetLookVector;

        public Vector3 TargetLookVector => _targetLookVector;
        
        private void Awake()
        {
            _joint = GetComponent<ConfigurableJoint>();
        }

        public void SetTargetLookFromPosition(Vector3 targetPosition)
        {
            var lookVector = transform.position - targetPosition;
            SetTargetLookVector(lookVector);
        }
        
        public void SetTargetLookVector(Vector3 lookVector)
        {
            lookVector.y = 0;
            var lookRotation = Quaternion.LookRotation(lookVector);

            _targetLookVector = lookVector;
            
            _joint.targetRotation = Quaternion.Inverse(lookRotation);
        }
    }
}