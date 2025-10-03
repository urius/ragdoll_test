using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Src.DebugComponents
{
    public class DrawGizmosComponent : MonoBehaviour
    {
        private static readonly List<DrawGizmoData> _gizmoDataList = new();
        
        public static void RequestDraw(string id, GizmoType gizmoType, Vector3 coords)
        {
            var targetGizmo = _gizmoDataList.FirstOrDefault(d => d.Id == id);

            if (targetGizmo == null)
            {
                targetGizmo = new DrawGizmoData(id);
                _gizmoDataList.Add(targetGizmo);
            }

            targetGizmo.GizmoType = gizmoType;
            targetGizmo.Coords = coords;
        }

        private void OnDrawGizmos()
        {
            foreach (var gizmoData in _gizmoDataList)
            {
                switch (gizmoData.GizmoType)
                {
                    case GizmoType.Sphere:
                        Gizmos.DrawSphere(gizmoData.Coords, 1);
                        break;
                    case GizmoType.Cube:
                        Gizmos.DrawCube(gizmoData.Coords, Vector3.one * 1); 
                        break;
                }
            }
        }
        
        private class DrawGizmoData
        {
            public readonly string Id;
            
            public GizmoType GizmoType;
            public Vector3 Coords;

            public DrawGizmoData(string id)
            {
                Id = id;
            }
        }
    }

    public enum GizmoType
    {
        Sphere,
        Cube,
    }
}