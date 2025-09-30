using Src.Data;
using UnityEngine;

namespace Src.Model
{
    public interface IFootballerUnit
    {
        FootballerBehaviourStrategy BehaviourStrategy { get; set; }
        TeamKey Team { get; }
        Vector3 Position { get; }

        void SetupData(TeamKey team, int teamInnerIndex);
        void SetTargetDirection(Vector3 directionVector);
        void SetMovingState();
        void SetMovingToTargetPointState(Vector3 targetPoint);
        void SetStandingState();
        bool IsOnTargetPoint();
    }
}