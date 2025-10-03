using Src.Providers;
using UnityEngine;

namespace Src.Data.BehaviourStates
{
    public class BringingBallCloserToEnemyGatesState : MoveToDynamicPositionStateBase
    {
        public BringingBallCloserToEnemyGatesState(Vector3 offset) : base(offset)
        {
        }

        public override BehaviourStateName StateName => BehaviourStateName.LeadTheBall;
    }
}