using UnityEngine;

namespace Src.Components
{
    public class SkinnedMeshColorSetter : MonoBehaviour, IColorSetter
    {
        [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshRenderers;
        
        public void SetColor(Color color)
        {
            if (_skinnedMeshRenderers == null) return;

            foreach (var renderer in _skinnedMeshRenderers)
            {
                if (renderer == null) continue;

                var propertyBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_BaseColor", color);
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}