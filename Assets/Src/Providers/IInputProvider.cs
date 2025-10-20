using System;
using UnityEngine;

namespace Src.Providers
{
    public interface IInputProvider
    {
        public event Action<Vector2> MoveStateChanged;
        public event Action<bool> AttackStateChanged;
        
        Vector2 MoveVectorNormalized { get; }
        bool IsAttacking { get; }
        bool IsSprinting { get; }
    }
}