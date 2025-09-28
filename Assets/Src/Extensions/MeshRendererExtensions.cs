using UnityEngine;

namespace Src.Extensions
{
    public static class MeshRendererExtensions
    {
        public static void SetColor(this MeshRenderer meshRenderer, Color color)
        {
            var propertyBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", color);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}