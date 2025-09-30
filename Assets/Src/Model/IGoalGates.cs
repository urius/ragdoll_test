using Src.Data;
using UnityEngine;

namespace Src.Model
{
    public interface IGoalGates
    {
        TeamKey Team { get; }
        Vector3 Position { get; }
        Vector3[] BoundPositions { get; }
        Vector3 Forward { get; }

        void SetupTeam(TeamKey teamKey);
    }
}