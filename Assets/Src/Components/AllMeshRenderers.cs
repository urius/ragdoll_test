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
                var propertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", color);
                meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}