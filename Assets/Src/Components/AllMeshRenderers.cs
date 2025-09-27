using Src.Extensions;
using UnityEngine;

namespace Src.Components
{
    public class AllMeshRenderers : MonoBehaviour
    {
        private MeshRenderer[] _allRenderers;

        private void Awake()
        {
            _allRenderers = GetComponentsInChildren<MeshRenderer>();
        }

        public void SetColor(Color color)
        {
            foreach (var meshRenderer in _allRenderers)
            {
                meshRenderer.SetColor(color);
            }
        }
    }
}