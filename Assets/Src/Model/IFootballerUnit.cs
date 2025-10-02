using Src.Data;
using UnityEngine;

namespace Src.Model
{
    public interface IFootballerUnit
    {
        FootballerRole Role { get; set; }
        TeamKey Team { get; }
        Vector3 Position { get; }
        Vector3 PositionProjected { get; }
        Vector3 ForwardProjected { get; }
        FootballerMoveState MoveState { get; }
        bool IsHittingTheBallAnimationPlaying { get; }

        void SetupData(TeamKey team, int teamInnerIndex);
        void SetTargetDirection(Vector3 directionVector);
        void SetMovingState();
        void SetMovingToTargetPointState(Vector3 targetPoint);
        void SetStandingState();
        void SetHittingBallStateRightLeg(Vector3 hitDirection);
        void SetHittingBallStateLeftLeg(Vector3 hitDirection);
        bool IsOnTargetPoint();
    }
}