using UnityEngine;

namespace Src.Extensions
{
    public static class MeshRendererExtensions
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        public static void SetColor(this MeshRenderer meshRenderer, Color color)
        {
            var propertyBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColor, color);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}