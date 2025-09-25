using UnityEngine;

namespace Src.Components
{
    public class FootsFrictionSetter : MonoBehaviour
    {
        [SerializeField] private Collider _leftFootCollider; 
        [SerializeField] private Collider _rightFootCollider;
        [SerializeField] private PhysicsMaterial _defaultFrictionMat;
        [SerializeField] private PhysicsMaterial _zeroFrictionMat;
            
        public void SetLeftFootFriction()
        {
            _leftFootCollider.sharedMaterial = _defaultFrictionMat;
            _rightFootCollider.sharedMaterial = _zeroFrictionMat;
        }
        
        public void SetRightFootFriction()
        {
            _rightFootCollider.sharedMaterial = _defaultFrictionMat;
            _leftFootCollider.sharedMaterial = _zeroFrictionMat;
        }
    }
}