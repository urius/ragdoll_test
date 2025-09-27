using Src.Extensions;
using UnityEngine;

namespace Src.Components
{
    public class BoundView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        public void SetColor(Color color)
        {
            _meshRenderer.SetColor(color);
        }
    }
}