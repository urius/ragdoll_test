using System;
using UnityEngine;

namespace Src.Components
{
    public class VelocityDamping : MonoBehaviour
    {
        [SerializeField] private float _verticalDampingFactor;
        [SerializeField] private float _horizontalDampingFactor;
        
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var linearVelocity = _rigidbody.linearVelocity;
            linearVelocity.y *= _verticalDampingFactor;
            linearVelocity.x *= _horizontalDampingFactor;
            linearVelocity.z *= _horizontalDampingFactor;
            _rigidbody.linearVelocity = linearVelocity;
        }

        public void SetHorizontalDampingFactor(float factor)
        {
            _horizontalDampingFactor = factor;
        }
    }
}