using Src.Data;
using UnityEngine;

namespace Src.Components
{
    public class StartPointsProvider : MonoBehaviour, IStartPointsProvider
    {
        [SerializeField] private Transform[] _points;

        public int PointsAmount => _points.Length;

        public Vector3 GetPointPosition(int pointIndex, TeamKey teamKey)
        {
            var result = Vector3.zero;

            if (pointIndex < _points.Length)
            {
                result = _points[pointIndex].position;
                if (teamKey == TeamKey.Blue)
                {
                    result.z = -result.z;
                }
            }

            return result;
        }
    }

    public interface IStartPointsProvider
    {
        int PointsAmount { get; }
        Vector3 GetPointPosition(int pointIndex, TeamKey teamKey);
    }
}