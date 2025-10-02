using System;
using UnityEngine;

namespace Src.Components
{
    public class FootCollisionNotifierComponent : MonoBehaviour
    {
        public event Action<BallFacade> BallCollisionEnter;

        private void OnCollisionEnter(Collision other)
        {
            var ballComponent = other.transform.GetComponent<BallFacade>();
            if (ballComponent != null)
            {
                BallCollisionEnter?.Invoke(ballComponent);
            }
        }
    }
}