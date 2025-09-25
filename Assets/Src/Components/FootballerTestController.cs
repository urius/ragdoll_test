using Src.Data;
using UnityEngine;

namespace Src.Components
{
    public class FootballerTestController : MonoBehaviour
    {
        [SerializeField] private Transform _targetMoveToTransform;
        private bool _isTargetTransformSet;
        
        private FootballerUnit _presenter;

        private void Awake()
        {
            _presenter = GetComponent<FootballerUnit>();
        }

        private void FixedUpdate()
        {
            if (_targetMoveToTransform != null)
            {
                _presenter.SetTargetMoveToPoint(_targetMoveToTransform.position);
                
                if (_isTargetTransformSet == false)
                {
                    _presenter.SetState(FootballerState.Moving);
                    _isTargetTransformSet = true;
                }
            }
            else if (_isTargetTransformSet)
            {
                _presenter.SetState(FootballerState.Standing);
                _isTargetTransformSet = false;
            }
        }
    }
}