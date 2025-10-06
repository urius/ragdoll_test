using UnityEngine;

namespace Src.Providers
{
    public class FieldBorderPositionProvider : IFieldBorderPositionProvider
    {
        private const int NearDistance = 20;
        private const int CloseDistance = 3;
        
        private readonly float[] _xBorders;
        private readonly float[] _zBorders;

        public FieldBorderPositionProvider(Transform fieldCornerTransform)
        {
            var fieldCornerPosition = fieldCornerTransform.position;
            
            _xBorders = new []
            {
               -fieldCornerPosition.x,
                fieldCornerPosition.x,
            };

            _zBorders = new []
            {
                -fieldCornerPosition.z,
                fieldCornerPosition.z,
            };
        }

        public Vector3 ClampByBorder(Vector3 coords)
        {
            var x = Mathf.Clamp(coords.x, _xBorders[0], _xBorders[1]);
            var z = Mathf.Clamp(coords.z, _zBorders[0], _zBorders[1]);

            return new Vector3(x, coords.y, z);
        }

        public bool IsNearLongBorder(Vector3 coords)
        {
            return coords.x < _xBorders[0] + NearDistance
                   || coords.x > _xBorders[1] - NearDistance;
        }

        public bool IsNearShortBorder(Vector3 coords)
        {
            return coords.z < _zBorders[0] + NearDistance
                   || coords.z > _zBorders[1] - NearDistance;
        }

        public bool IsCloseToShortBorder(Vector3 coords)
        {
            return coords.z < _zBorders[0] + CloseDistance
                   || coords.z > _zBorders[1] - CloseDistance;
        }

        public bool IsNearAnyBorder(Vector3 coords)
        {
            return IsNearLongBorder(coords) || IsNearShortBorder(coords);
        }

        public bool IsNearCorner(Vector3 coords)
        {
            return IsNearLongBorder(coords) && IsNearShortBorder(coords);
        }
    }
}