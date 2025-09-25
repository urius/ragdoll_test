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
        
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int IsIdle = Animator.StringToHash("IsIdle");
        
        private FootballerUnitData _unitData;

        public Vector3 TargetMoveToPoint { get; private set; }
        public FootballerState State { get; private set; }
        public FootballerUnitData UnitData => _unitData;

        public void SetupData(TeamKey team, int teamInnerIndex)
        {
            _unitData.Team = team;
            _unitData.TeamInnerIndex = teamInnerIndex;
        }

        public void SetTargetMoveToPoint(Vector3 targetPoint)
        {
            TargetMoveToPoint = targetPoint;
            
            _lookToBehaviour.SetTargetLookFromPosition(TargetMoveToPoint);
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
                    break;
                case FootballerState.Standing:
                    _velocityDamping.SetHorizontalDampingFactor(0.2f);
                    _animator.SetTrigger(IsIdle);
                    break;
            }
        }
    }
}