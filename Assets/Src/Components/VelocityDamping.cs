using System;
using UnityEngine;

namespace Src.Components
{
    public class VelocityDamping : MonoBehaviour
    {
        [SerializeField] private float _verticalDampingFactorUp;
        [SerializeField] private float _verticalDampingFactorDown;
        [SerializeField] private float _horizontalDampingFactor;
        
        private Rigidbody _rigidbody;

        public float VerticalDampingFactorUp => _verticalDampingFactorUp;
        public float VerticalDampingFactorDown => _verticalDampingFactorDown;
        public float HorizontalDampingFactor => _horizontalDampingFactor;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var linearVelocity = _rigidbody.linearVelocity;
            linearVelocity.y *= linearVelocity.y > 0 ? _verticalDampingFactorUp : _verticalDampingFactorDown;
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