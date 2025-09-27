using Src.Data;
using UnityEngine;

namespace Src.Model
{
    public interface IGoalGates
    {
        TeamKey Team { get; }
        Vector3 Position { get; }

        void SetupTeam(TeamKey teamKey);
    }
}