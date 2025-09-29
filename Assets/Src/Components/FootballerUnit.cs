using System;
using Src.Data;
using Src.Model;
using UnityEngine;

namespace Src.Components
{
    public class FootballerUnit : MonoBehaviour, IFootballerUnit
    {
        [SerializeField] private LookToBehaviour _lookToBehaviour;
        [SerializeField] private Animator _animator;
        [SerializeField] private VelocityDamping _velocityDamping;
        [SerializeField] private Rigidbody _mainRigidbody;
        [SerializeField] private AllMeshRenderers _allMeshRenderers;
        [SerializeField] private Color _redTeamColor;
        [SerializeField] private Color _blueTeamColor;
        
        private const float DeltaVelocity = 0.2f;
        
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int IsIdle = Animator.StringToHash("IsIdle");

        private float _defaultHorizontalDampingFactor = 0.9f;
        private FootballerUnitData _unitData;
        private float _targetVelocity = 0;
        private float _currentVelocity = 0;

        public Vector3 TargetMoveToPoint { get; private set; }
        public FootballerState State { get; private set; }
        public FootballerUnitData UnitData => _unitData;
        
        private Vector3 ProjectedForward
        {
            get
            {
                var result = _mainRigidbody.transform.forward;
                result.y = 0;
                return result;
            }
        }

        private void Awake()
        {
            _defaultHorizontalDampingFactor = _velocityDamping.HorizontalDampingFactor;
        }

        private void FixedUpdate()
        {
            DecreaseVelocityIfNeeded();
            
            if (_targetVelocity > 0)
            {
                var projectedFroward = ProjectedForward;
                var angle = Vector3.Angle(_lookToBehaviour.TargetLookVector, projectedFroward);
                if (angle < 90)
                {
                    IncreaseVelocityIfNeeded();
                    
                    _mainRigidbody.linearVelocity =
                        _currentVelocity > 0 ? ProjectedForward * _currentVelocity : Vector3.zero;
                }
            }
        }

        public void SetupData(TeamKey team, int teamInnerIndex)
        {
            _unitData.Team = team;
            SetupColorFromTeam();
            _unitData.TeamInnerIndex = teamInnerIndex;
        }

        public void SetTargetDirection(Vector3 directionVector)
        {
            _lookToBehaviour.SetTargetLookVector(directionVector);
        }

        public void SetTargetMoveToPoint(Vector3 targetPoint)
        {
            TargetMoveToPoint = targetPoint;
            
            _lookToBehaviour.SetTargetLookFromPosition(TargetMoveToPoint);
        }

        public void SetMovingState()
        {
            SetState(FootballerState.Moving);
        }

        public void SetStandingState()
        {
            SetState(FootballerState.Standing);
        }

        public void SetState(FootballerState state)
        {
            if (State == state) return;
            
            State = state;
            
            switch (state)
            {
                case FootballerState.Moving:
                    _velocityDamping.SetHorizontalDampingFactor(1);
                    _animator.SetTrigger(IsRunning);
                    _targetVelocity = 15f;
                    break;
                case FootballerState.Standing:
                    _velocityDamping.SetHorizontalDampingFactor(_defaultHorizontalDampingFactor);
                    _animator.SetTrigger(IsIdle);
                    _targetVelocity = 0f;
                    break;
                default:
                    _velocityDamping.SetHorizontalDampingFactor(_defaultHorizontalDampingFactor);
                    break;
            }
        }

        private void IncreaseVelocityIfNeeded()
        {
            if (_currentVelocity < _targetVelocity)
            {
                _currentVelocity += DeltaVelocity;
                ClampVelocity();
            }
        }

        private void DecreaseVelocityIfNeeded()
        {
            if (_targetVelocity <= 0 && _currentVelocity > 0)
            {
                _currentVelocity -= DeltaVelocity;
                ClampVelocity();
            }
        }

        private void ClampVelocity()
        {
            _currentVelocity = Math.Clamp(_currentVelocity, 0, _targetVelocity);
        }

        private void SetupColorFromTeam()
        {
            switch (_unitData.Team)
            {
                case TeamKey.Red:
                    _allMeshRenderers.SetColor(_redTeamColor);
                    break;
                case TeamKey.Blue:
                    _allMeshRenderers.SetColor(_blueTeamColor);
                    break;
            }
        }
    }
}