using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using IInitializable = VContainer.Unity.IInitializable;

namespace Src.Providers
{
    public class InputHandler : IInputProvider, IInitializable, IDisposable
    {
        public event Action<Vector2> MoveStateChanged;
        public event Action<bool> AttackStateChanged;
        public event Action<bool> SprintStateChanged;
        
        private readonly InputActionsSource _inputSource;
        
        private Vector2 _moveVectorNormalized;
        private bool _isAttacking;
        private bool _isSprinting;

        public InputHandler()
        {
            _inputSource = new InputActionsSource();
            _inputSource.Enable();
        }

        public Vector2 MoveVectorNormalized => _moveVectorNormalized;
        public bool IsAttacking => _isAttacking;
        public bool IsSprinting => _isSprinting;

        public void Initialize()
        {
            _inputSource.Player.Move.performed += OnMovePerformed;
            _inputSource.Player.Move.canceled += OnMoveCancelled;
            _inputSource.Player.Attack.performed += OnAttack;
            _inputSource.Player.Attack.canceled += OnAttackCancelled;
            _inputSource.Player.Sprint.performed += OnSprint;
            _inputSource.Player.Sprint.canceled += OnSprintCancelled;
        }

        public void Dispose()
        {
            _inputSource.Player.Move.performed -= OnMovePerformed;
            _inputSource.Player.Move.canceled -= OnMoveCancelled;
            _inputSource.Player.Attack.performed -= OnAttack;
            _inputSource.Player.Attack.canceled -= OnAttackCancelled;
            _inputSource.Player.Sprint.performed -= OnSprint;
            _inputSource.Player.Sprint.canceled -= OnSprintCancelled;

            _inputSource.Dispose();
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveVectorNormalized = ctx.ReadValue<Vector2>();
            MoveStateChanged?.Invoke(_moveVectorNormalized);
        }

        private void OnMoveCancelled(InputAction.CallbackContext ctx)
        {
            _moveVectorNormalized = Vector3.zero;
            MoveStateChanged?.Invoke(_moveVectorNormalized);
        }

        private void OnAttack(InputAction.CallbackContext ctx)
        {
            _isAttacking = true;
            Debug.Log("OnAttack " + _isAttacking);
            
            AttackStateChanged?.Invoke(_isAttacking);
        }

        private void OnAttackCancelled(InputAction.CallbackContext ctx)
        {
            _isAttacking = false;
            AttackStateChanged?.Invoke(_isAttacking);
        }

        private void OnSprint(InputAction.CallbackContext ctx)
        {
            _isSprinting = true;
            SprintStateChanged?.Invoke(_isSprinting);
        }

        private void OnSprintCancelled(InputAction.CallbackContext ctx)
        {
            _isSprinting = false;
            SprintStateChanged?.Invoke(_isSprinting);
        }
    }
}