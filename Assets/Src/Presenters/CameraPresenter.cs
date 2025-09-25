using System;
using Src.Components;
using Src.Model;
using Src.Providers;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Src.Presenters
{
    public class CameraPresenter : MonoBehaviour, ICameraDirectionProvider
    {
        [SerializeField] private Camera _camera;

        private const float Sensitivity = 1;
        
        private PlayerControlledUnitProvider _playerControlledUnitProvider;
        private FootballerUnit _targetUnit;
        private bool _targetUnitIsLocked;

        public Vector3 Forward => transform.forward;

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
                RotateCameraToMouse();
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
                transform.position = Vector3.Lerp(transform.position, _targetUnit.transform.position, 5 * Time.deltaTime);

                var targetRotation = _targetUnit.transform.rotation.eulerAngles;
                targetRotation.y += 180;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), 3 * Time.deltaTime);
                
                if ((_targetUnit.transform.position - transform.position).sqrMagnitude < 0.3f 
                    && Math.Abs(transform.rotation.eulerAngles.y - targetRotation.y) < 0.5f)
                {
                    _targetUnitIsLocked = true;
                }
            }
        }
        
        private void RotateCameraToMouse()
        {
            if (_targetUnitIsLocked == false) return;
            
            var mouse = Mouse.current;
            
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