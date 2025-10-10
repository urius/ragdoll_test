using UnityEngine;

namespace Src.Providers
{
    public interface IFieldBorderPositionProvider
    {
        public Vector3 ClampByBorder(Vector3 coords, float offset = 0);
        public bool IsNearLongBorder(Vector3 coords);
        public bool IsNearShortBorder(Vector3 coords);
        public bool IsCloseToShortBorder(Vector3 coords);
        public bool IsCloseToLongBorder(Vector3 coords);
        public bool IsNearAnyBorder(Vector3 coords);
        public bool IsNearCorner(Vector3 coords);
        public float GetLeftQuarterX();
        public float GetRightQuarterX();
    }
}