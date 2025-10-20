using System;
using Src.Components;
using Src.Model;
using Src.Providers;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Src.Presenters
{
    public class CameraContainerPresenter : MonoBehaviour, ICameraDirectionProvider
    {
        [SerializeField] private Camera _camera;

        private const float Sensitivity = 1;
        
        private PlayerControlledUnitProvider _playerControlledUnitProvider;
        private FootballerUnit _targetUnit;
        private bool _targetUnitIsLocked;

        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;

        [Inject]
        public void Setup(PlayerControlledUnitProvider playerControlledUnitProvider)
        {
            _playerControlledUnitProvider = playerControlledUnitProvider;

            Subscribe();
            RefreshTargetUnit();
        }
        
        private void Update()
        {
            if (_targetUnit != null)
            {
                MoveToTarget();

                if (_targetUnitIsLocked)
                {
                    RotateCameraToMouse();
                }
            }
        }

        private void MoveToTarget()
        {
            if (_targetUnitIsLocked)
            {
                transform.position = _targetUnit.transform.position;
            }
            else
            {
                var targetUnitTransform = _targetUnit.transform;
                var cameraContainerTransform = transform;
                
                var moveVector = targetUnitTransform.position - cameraContainerTransform.position;
                var sqrMagnitude = moveVector.sqrMagnitude;
                
                var targetRotation = targetUnitTransform.rotation.eulerAngles;
                targetRotation.x = targetRotation.z = 0;
                transform.rotation = Quaternion.Lerp(cameraContainerTransform.rotation, Quaternion.Euler(targetRotation), 3 * Time.deltaTime);
                
                if (sqrMagnitude >= 1)
                {
                    transform.position = Vector3.Lerp(transform.position, _targetUnit.transform.position, 6 * Time.deltaTime);
                }
                else
                {
                    var distance = moveVector.magnitude;
                    var moveSpeed = 3 * Time.deltaTime;

                    transform.position += moveVector.normalized * (distance < moveSpeed ? distance : moveSpeed);
                
                    if (sqrMagnitude < 0.05f)
                    {
                        _targetUnitIsLocked = true;
                    }
                }
            }
        }
        
        private void RotateCameraToMouse()
        {
            var mouse = Mouse.current;
            var mouseDeltaX = mouse.delta.x.value;
            
            if (Mathf.Abs(mouseDeltaX) > 100) return;
            
            var eulerRotation = transform.localRotation.eulerAngles;
            eulerRotation.y += mouse.delta.x.value * Sensitivity;
            transform.localRotation = Quaternion.Euler(eulerRotation);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerControlledUnitProvider.TargetUnitChanged += OnTargetUnitChanged;
        }

        private void Unsubscribe()
        {
            _playerControlledUnitProvider.TargetUnitChanged -= OnTargetUnitChanged;
        }

        private void OnTargetUnitChanged(IFootballerUnit _)
        {
            RefreshTargetUnit();
        }

        private void RefreshTargetUnit()
        {
            _targetUnit = _playerControlledUnitProvider.TargetUnit as FootballerUnit;
            _targetUnitIsLocked = false;
            
            Cursor.lockState = _targetUnit != null ? CursorLockMode.Locked : CursorLockMode.None;
        }

    }
}