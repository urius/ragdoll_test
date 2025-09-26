using Src.Data;
using UnityEngine;

namespace Src.Model
{
    public interface IFootballerUnit
    {
        FootballerUnitData UnitData { get; }

        void SetupData(TeamKey team, int teamInnerIndex);
        void SetTargetDirection(Vector3 directionVector);
        void SetMovingState();
        void SetStandingState();
    }
}