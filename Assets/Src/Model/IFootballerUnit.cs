using Src.Data;
using Src.Data.BehaviourStates;
using Src.Providers;
using UnityEngine;

namespace Src.Model
{
    public interface IFootballerUnit : IDynamicPositionProvider
    {
        FootballerRole Role { get; set; }
        TeamKey Team { get; }
        Vector3 ForwardProjected { get; }
        FootballerMoveState MoveState { get; }
        bool IsHittingTheBallAnimationPlaying { get; }
        BehaviourStateName BehaviourState { get; }

        void SetupData(TeamKey team, int teamInnerIndex);

        void SetInterceptBallState(Vector3 offset);
        void SetLeadTheBallState();
        void ResetBehaviourState();
        void SetPlayerControlledBehaviourState();
        bool IsOnTargetPoint();

        void SetTargetDirection(Vector3 directionVector);
        void SetMovingState();
        void SetMovingToTargetPointState(Vector3 targetPoint);
        void SetStandingState();
        void SetHittingBallStateRightLeg(Vector3 hitDirection);
        void SetHittingBallStateLeftLeg(Vector3 hitDirection);
    }
}