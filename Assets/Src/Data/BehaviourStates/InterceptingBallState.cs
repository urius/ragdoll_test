using UnityEngine;

namespace Src.Data.BehaviourStates
{
    public class InterceptingBallState : MoveToDynamicPositionStateBase
    {
        public InterceptingBallState(Vector3 offset) : base(offset)
        {
        }

        public override BehaviourStateName StateName => BehaviourStateName.InterceptingBall;
    }
}