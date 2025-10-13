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
        public bool TargetPointWasReached { get; }

        void SetupData(TeamKey team, int teamInnerIndex);
        
        void ChangeRole(FootballerRole role);

        void SetLeadTheBallState();
        void SetMoveToTargetPointState(Vector3 targetPoint);
        void ResetBehaviourState();
        void SetPlayerControlledBehaviourState();
        bool IsOnTargetPoint();

        void SetTargetDirection(Vector3 directionVector);
        void SetMovingState();
        void SetStandingState();
        void SetHittingBallState(Vector3 hitDirection, float strengthHorizontal = 50, float strengthVertical = 5);
        void SetMaxSpeed(int maxSpeed);
        void RequestCorrectBallSpeed(Vector3 vector3);
    }
}