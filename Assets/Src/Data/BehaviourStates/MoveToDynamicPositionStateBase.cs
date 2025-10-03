using UnityEngine;

namespace Src.Data.BehaviourStates
{
    public abstract class MoveToDynamicPositionStateBase : BehaviourStateBase
    {
        public readonly Vector3 Offset;

        protected MoveToDynamicPositionStateBase(Vector3 offset)
        {
            Offset = offset;
        }
    }
}