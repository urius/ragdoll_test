using System;
using UnityEngine;

namespace Src.Components
{
    public class TargetJointsRotationSetter : MonoBehaviour
    {
        [SerializeField] private TransformJointPair[] _pairs;

        private void FixedUpdate()
        {
            for (var i = 0; i < _pairs.Length; i++)
            {
                var transformJointPair = _pairs[i];
                
                transformJointPair.ConfigurableJoint.targetRotation =
                    Quaternion.Inverse(transformJointPair.Transform.localRotation);
            }
        }

        [Serializable]
        private struct TransformJointPair
        {
            public Transform Transform;
            public ConfigurableJoint ConfigurableJoint;
        }
    }

}