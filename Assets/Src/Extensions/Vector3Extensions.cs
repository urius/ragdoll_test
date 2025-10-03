using UnityEngine;

namespace Src.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 Projected(this Vector3 original)
        {
            original.y = 0;
            return original;
        }
    }
}