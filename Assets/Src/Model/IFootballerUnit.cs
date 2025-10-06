using System;
using Src.Data;
using Src.Data.BehaviourStates;
using Src.Providers;
using UnityEngine;

namespace Src.Model
{
    public interface IFootballerUnit : IDynamicPositionProvider
    {
        event Action<IFootballerUnit> MovedToTargetPoint;
        
        FootballerRole Role { get; }
        TeamKey Team { get; }
        BehaviourStateName BehaviourState { get; }
        Vector3 TargetMoveToPoint { get; }

        void SetupData(TeamKey team, int teamInnerIndex);
        
        void ChangeRole(FootballerRole role);

        void SetInterceptBallState(Vector3 offset);
        void SetLeadTheBallState();
        void SetMoveToTargetPointState(Vector3 targetPoint);
        void ResetBehaviourState();
        void SetPlayerControlledBehaviourState();
        bool IsOnTargetPoint();

        void SetTargetDirection(Vector3 directionVector);
        void SetMovingState();
        void SetStandingState();
        void SetHittingBallState(Vector3 hitDirection);
        void SetMaxSpeed(int maxSpeed);
    }
}