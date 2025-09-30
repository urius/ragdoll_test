using System.Linq;
using Src.Components;
using Src.Data;
using Src.Model;
using UnityEngine;
using VContainer.Unity;

namespace Src.Providers
{
    public class GoalGatesProvider : IStartable, IGoalGatesProvider
    {
        public IGoalGates[] GoalGates { get; private set; }
        
        public IGoalGates GetGatesForTeam(TeamKey team)
        {
            foreach (var goalGate in GoalGates)
            {
                if (goalGate.Team == team)
                {
                    return goalGate;
                }
            }

            return null;
        }

        public void Start()
        {
            GoalGates = Object.FindObjectsByType<GoalGatesFacade>(FindObjectsSortMode.None)
                .Select(g => g as IGoalGates)
                .ToArray();
            
            Debug.Log("GoalGates len: " + GoalGates.Length);
        }
    }

    public interface IGoalGatesProvider
    {
        IGoalGates[] GoalGates { get; }
        IGoalGates GetGatesForTeam(TeamKey team);
    }
}