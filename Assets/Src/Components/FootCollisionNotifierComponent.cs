using System;
using UnityEngine;

namespace Src.Components
{
    public class FootCollisionNotifierComponent : MonoBehaviour
    {
        public event Action<BallModelFacade> BallCollisionEnter;

        private void OnCollisionEnter(Collision other)
        {
            var ballComponent = other.transform.GetComponent<BallModelFacade>();
            if (ballComponent != null)
            {
                BallCollisionEnter?.Invoke(ballComponent);
            }
        }
    }
}