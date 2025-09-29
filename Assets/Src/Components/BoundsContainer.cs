using UnityEngine;

namespace Src.Components
{
    public class BoundsContainer : MonoBehaviour
    {
        [SerializeField] private Color _redBoundsColor;
        [SerializeField] private Color _blueBoundsColor;
        
        [SerializeField] private BoundView[] _redBounds;
        [SerializeField] private BoundView[] _blueBounds;

        private void Awake()
        {
            SetBoundColors();
        }

        private void SetBoundColors()
        {
            foreach (var bound in _redBounds)
            {
                bound.SetColor(_redBoundsColor);   
            }
            
            foreach (var bound in _blueBounds)
            {
                bound.SetColor(_blueBoundsColor);    
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            SetBoundColors();
        }
    }
}