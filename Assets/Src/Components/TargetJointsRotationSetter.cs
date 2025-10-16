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
                    Quaternion.Inverse(GetLocalRotation(transformJointPair));
            }
        }

        private static Quaternion GetLocalRotation(TransformJointPair jointPairData)
        {
            if (jointPairData.LocalRotationRetrieveMode == LocalRotationRetrieveMode.XtoZ)
            {
                var eulerOriginal = jointPairData.Transform.localRotation.eulerAngles;
                return Quaternion.Euler(0, 0, eulerOriginal.x);
            }
            else if (jointPairData.LocalRotationRetrieveMode == LocalRotationRetrieveMode.XtoNegX)
            {
                var eulerOriginal = jointPairData.Transform.localRotation.eulerAngles;
                return Quaternion.Euler(-eulerOriginal.x, 0, 0);
            }

            return jointPairData.Transform.localRotation;
        }

        [Serializable]
        private struct TransformJointPair
        {
            public Transform Transform;
            public ConfigurableJoint ConfigurableJoint;
            public LocalRotationRetrieveMode LocalRotationRetrieveMode;
        }
        
        private enum LocalRotationRetrieveMode
        {
            Default,
            XtoZ,
            XtoNegX,
        }
    }

}