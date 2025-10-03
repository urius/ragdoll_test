using System;
using Src.Data;
using Src.Data.BehaviourStates;
using Src.Extensions;
using Src.Model;
using Src.Providers;
using UnityEngine;
using VContainer;

namespace Src.Components
{
    public class FootballerUnit : MonoBehaviour, IFootballerUnit
    {
        [SerializeField] private LookToBehaviour _lookToBehaviour;
        [SerializeField] private Animator _animator;
        [SerializeField] private VelocityDamping _velocityDamping;
        [SerializeField] private Rigidbody _mainRigidbody;
        [SerializeField] private AllMeshRenderers _allMeshRenderers;
        [SerializeField] private HitTheBallState _hitTheBallState;
        [SerializeField] private Color _redTeamColor;
        [SerializeField] private Color _blueTeamColor;
        [SerializeField] private FootCollisionNotifierComponent[] _footCollisionNotifiers;
        
        private const float DeltaVelocity = 0.2f;

        private static readonly int MoveStateParamKey = Animator.StringToHash("MoveState");
        private static readonly int IsIdle = 0;
        private static readonly int IsRunning = 1;
        private static readonly int IsHittingTheBallRight = 2;
        private static readonly int IsHittingTheBallLeft = 3;

        private float _defaultHorizontalDampingFactor = 0.9f;
        private FootballerUnitData _unitData;
        private float _targetVelocity = 0;
        private float _currentVelocity = 0;
        private FootballerMoveState _nextMoveState = FootballerMoveState.Standing;
        private IBallPositionProvider _ballPositionProvider;
        private Vector3 _targetMoveToOffset;

        public FootballerRole Role { get; set; }
        public Vector3 TargetMoveToPoint { get; private set; }
        public Vector3 ForwardProjected {
            get
            {
                var result = transform.forward;
                result.y = 0;
                return result;
            }
        }
        public BehaviourStateName BehaviourState { get; private set; }
        public FootballerMoveState MoveState { get; private set; }
        public TeamKey Team => _unitData.Team;
        public Vector3 Position => transform.position;
        public Vector3 PositionProjected
        {
            get
            {
                var result = transform.position;
                result.y = 0;
                return result;
            }
        }

        public bool IsHittingTheBallAnimationPlaying => _hitTheBallState.IsHitting;

        private Vector3 ProjectedForward
        {
            get
            {
                var result = _mainRigidbody.transform.forward;
                result.y = 0;
                return result;
            }
        }

        private Vector3 _requestedHitDirection;

        private void Awake()
        {
            _defaultHorizontalDampingFactor = _velocityDamping.HorizontalDampingFactor;

            Subscribe();
        }

        private void FixedUpdate()
        {
            DecreaseVelocityIfNeeded();
            
            if (_targetVelocity > 0)
            {
                _hitTheBallState.ResetHitState();
                
                var projectedFroward = ProjectedForward;
                var angle = Vector3.Angle(_lookToBehaviour.TargetLookVector, projectedFroward);
                if (angle < 35)
                {
                    IncreaseVelocityIfNeeded();
                    
                    _mainRigidbody.linearVelocity =
                        _currentVelocity > 0 ? ProjectedForward * _currentVelocity : Vector3.zero;
                }

                ProcessMoveFinishedIfNeeded();
            }

            ProcessBehaviourState();
        }

        [Inject]
        public void Construct(IBallPositionProvider ballPositionProvider)
        {
            _ballPositionProvider = ballPositionProvider;
        }

        public void SetupData(TeamKey team, int teamInnerIndex)
        {
            _unitData.Team = team;
            SetupColorFromTeam();
            _unitData.TeamInnerIndex = teamInnerIndex;
        }

        public void SetInterceptBallState(Vector3 offset)
        {
            _targetMoveToOffset = offset;
            if (BehaviourState == BehaviourStateName.InterceptingBall) return;
            
            BehaviourState = BehaviourStateName.InterceptingBall;
            ProcessBehaviourState();
        }

        public void ResetBehaviourState()
        {
            BehaviourState = BehaviourStateName.Undefined;
        }

        public void SetPlayerControlledBehaviourState()
        {
            if (BehaviourState == BehaviourStateName.PlayerControlled) return;
            
            BehaviourState = BehaviourStateName.PlayerControlled;
            SetStandingState();
        }

        public void SetLeadTheBallState()
        {
            _targetMoveToOffset = Vector3.zero;
            if (BehaviourState == BehaviourStateName.LeadTheBall) return;

            BehaviourState = BehaviourStateName.LeadTheBall;
            ProcessBehaviourState();
        }

        private void ProcessBehaviourState()
        {
            switch (BehaviourState)
            {
                case BehaviourStateName.Undefined:
                    SetStandingState();
                    break;
                case BehaviourStateName.InterceptingBall:
                case BehaviourStateName.LeadTheBall:
                    SetTargetMoveToPoint((_ballPositionProvider.Position + _targetMoveToOffset).Projected());
                    SetMovingState();
                    break;
            }
        }

        public void SetTargetDirection(Vector3 directionVector)
        {
            _lookToBehaviour.SetTargetLookVector(directionVector);
        }

        public void SetMovingToTargetPointState(Vector3 targetPoint)
        {
            SetTargetMoveToPoint(targetPoint);
            
            if (IsOnTargetPoint()) return;
            
            SetMovingState();
        }

        public void SetTargetMoveToPoint(Vector3 targetPoint)
        {
            TargetMoveToPoint = targetPoint;
            
            _lookToBehaviour.SetTargetLookFromPosition(TargetMoveToPoint);
        }

        public void SetMovingState()
        {
            SetState(FootballerMoveState.Moving);
        }

        public void SetStandingState()
        {
            SetState(FootballerMoveState.Standing);
        }

        public void SetHittingBallStateRightLeg(Vector3 hitDirection)
        {
            _requestedHitDirection = hitDirection;
            SetState(FootballerMoveState.HittingTheBallRight);
        }

        public void SetHittingBallStateLeftLeg(Vector3 hitDirection)
        {
            _requestedHitDirection = hitDirection;
            SetState(FootballerMoveState.HittingTheBallLeft);
        }

        public void SetState(FootballerMoveState moveState, bool force = false)
        {
            if (MoveState == moveState) return;

            if (force == false 
                && MoveState is FootballerMoveState.HittingTheBallRight or FootballerMoveState.HittingTheBallLeft)
            {
                _nextMoveState = moveState;
                return;
            }
            
            MoveState = moveState;
            
            switch (moveState)
            {
                case FootballerMoveState.Moving:
                    _velocityDamping.SetHorizontalDampingFactor(1);
                    _hitTheBallState.ResetHitState();
                    _animator.SetInteger(MoveStateParamKey, IsRunning);
                    _targetVelocity = 15f;
                    break;
                case FootballerMoveState.Standing:
                    _velocityDamping.SetHorizontalDampingFactor(_defaultHorizontalDampingFactor);
                    _hitTheBallState.ResetHitState();
                    _animator.SetInteger(MoveStateParamKey, IsIdle);
                    _targetVelocity = 0f;
                    break;
                case FootballerMoveState.HittingTheBallRight:
                    _animator.SetInteger(MoveStateParamKey, IsHittingTheBallRight);
                    _targetVelocity = 0f;
                    break;
                case FootballerMoveState.HittingTheBallLeft:
                    _animator.SetInteger(MoveStateParamKey, IsHittingTheBallLeft);
                    _targetVelocity = 0f;
                    break;
                default:
                    _hitTheBallState.ResetHitState();
                    _velocityDamping.SetHorizontalDampingFactor(_defaultHorizontalDampingFactor);
                    break;
            }
        }

        public bool IsOnTargetPoint()
        {
            return GetFlatDistance(TargetMoveToPoint, transform.position) < 1f;
        }

        private void Subscribe()
        {
            _hitTheBallState.HitTheBallAnimationEnded += OnHitTheBallAnimationEnded;
            foreach (var footCollisionNotifier in _footCollisionNotifiers)
            {
                footCollisionNotifier.BallCollisionEnter += OnBallCollisionEnter;
            }
        }

        private void OnBallCollisionEnter(BallFacade ballFacade)
        {
            if (_hitTheBallState.IsHitting)
            {
                ballFacade.SetLinearVelocity(_requestedHitDirection);
            }
        }

        private void OnHitTheBallAnimationEnded()
        {
            SetState(_nextMoveState, force: true);
            _nextMoveState = FootballerMoveState.Standing;
        }

        private void ProcessMoveFinishedIfNeeded()
        {
            if (MoveState == FootballerMoveState.Moving 
                && IsOnTargetPoint())
            {
                SetState(FootballerMoveState.Standing);
            }
        }

        private double GetFlatDistance(Vector3 targetMoveToPoint, Vector3 transformPosition)
        {
            targetMoveToPoint.y = transformPosition.y = 0;

            return Vector3.Distance(targetMoveToPoint, transformPosition);
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