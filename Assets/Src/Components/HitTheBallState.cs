using System;
using UnityEngine;

namespace Src.Components
{
    public class HitTheBallState : MonoBehaviour
    {
        public event Action HitTheBallAnimationEnded;
        
        private bool _isHitting;

        public bool IsHitting => _isHitting;

        public void SetIsHit()
        {
            _isHitting = true;
        }
        
        public void SetIsNoHit()
        {
            _isHitting = false;
            
            HitTheBallAnimationEnded?.Invoke();
        }

        public void ResetHitState()
        {
            _isHitting = false;
        }
    }
}