using UnityEngine;

namespace Src.Components
{
    public class HideAllChildrenMeshRenderers : MonoBehaviour
    {
        private void Awake()
        {
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.enabled = false;
            }
        }
    }
}