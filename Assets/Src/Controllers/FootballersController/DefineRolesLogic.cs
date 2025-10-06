using System.Collections.Generic;
using Src.Data;
using Src.Model;
using Src.Providers;
using UnityEngine;

namespace Src.Controllers.FootballersController
{
    public class DefineRolesLogic
    {
        private readonly IGameUnitsProvider _unitsProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;
        private readonly IBallPositionProvider _ballPositionProvider;

        private readonly Dictionary<TeamKey, IFootballerUnit> _unitByTeamBufferDict = new();
        private readonly Dictionary<TeamKey,List<FootballerData>> _unitOperationalDataByTeam = new();

        public DefineRolesLogic(
            IGameUnitsProvider unitsProvider,
            IGoalGatesProvider goalGatesProvider,
            IBallPositionProvider ballPositionProvider)
        {
            _unitsProvider = unitsProvider;
            _goalGatesProvider = goalGatesProvider;
            _ballPositionProvider = ballPositionProvider;
        }

        public void DefineGoalkeepers()
        {
            _unitByTeamBufferDict.Clear();
            var closestToGoalGatesUnits = _unitByTeamBufferDict;

            foreach (var footballerUnit in _unitsProvider.Footballers)
            {
                var team = footballerUnit.Team;
                if (closestToGoalGatesUnits.ContainsKey(team) == false)
                {
                    closestToGoalGatesUnits[team] = footballerUnit;
                    continue;
                }

                var goalGatesPosition = _goalGatesProvider.GetGatesForTeam(team).Position;
                if (Vector3.Distance(goalGatesPosition, footballerUnit.Position) <
                    Vector3.Distance(goalGatesPosition, closestToGoalGatesUnits[team].Position))
                {
                    closestToGoalGatesUnits[team] = footballerUnit;
                }
            }

            foreach (var kvp in closestToGoalGatesUnits)
            {
                kvp.Value.ChangeRole(FootballerRole.Goalkeeper);
            }
        }

        public void UpdateRoles()
        {
            var ballPosition = _ballPositionProvider.Position;
            
            ResetRoleFlagsAndUpdateUnitDistances();
            var closestToBallUnitDataRed = GetMinDistanceForNotDefinedRoleUnit(ballPosition, TeamKey.Red);
            SetRole(closestToBallUnitDataRed, FootballerRole.Attacker);
            var closestToBallUnitDataBlue = GetMinDistanceForNotDefinedRoleUnit(ballPosition, TeamKey.Blue);
            SetRole(closestToBallUnitDataBlue, FootballerRole.Attacker);
            
            var secondaryClosestToBallUnitDataRed = GetMinDistanceForNotDefinedRoleUnit(ballPosition, TeamKey.Red);
            SetRole(secondaryClosestToBallUnitDataRed, FootballerRole.AttackerSupport);
            var secondaryClosestToBallUnitDataBlue = GetMinDistanceForNotDefinedRoleUnit(ballPosition, TeamKey.Blue);
            SetRole(secondaryClosestToBallUnitDataBlue, FootballerRole.AttackerSupport);
            
            foreach (var unitData in _unitOperationalDataByTeam[TeamKey.Red])
            {
                if (unitData.IsUnitRoleDefined) continue;

                unitData.FootballerUnit.ChangeRole(FootballerRole.Defender);
            }

            foreach (var unitData in _unitOperationalDataByTeam[TeamKey.Blue])
            {
                if (unitData.IsUnitRoleDefined) continue;
                
                unitData.FootballerUnit.ChangeRole(FootballerRole.Defender);
            }
        }

        private static void SetRole(FootballerData unitData, FootballerRole role)
        {
            unitData.FootballerUnit.ChangeRole(role);
            unitData.IsUnitRoleDefined = true;
        }

        private FootballerData GetMinDistanceForNotDefinedRoleUnit(Vector3 targetPosition, TeamKey teamKey)
        {
            FootballerData result = null;

            foreach (var unitData in _unitOperationalDataByTeam[teamKey])
            {
                var footballerUnit = unitData.FootballerUnit;
                if (unitData.IsUnitRoleDefined) continue;
                if (result == null)
                {
                    result = unitData;
                    continue;
                }

                var distance = Vector3.Distance(targetPosition, footballerUnit.Position);
                
                if (distance < Vector3.Distance(targetPosition, result.FootballerUnit.Position))
                {
                    result = unitData;
                }
            }

            return result;
        }

        private void ResetRoleFlagsAndUpdateUnitDistances()
        {
            if (_unitOperationalDataByTeam.Count <= 0)
            {
                _unitOperationalDataByTeam[TeamKey.Red] = new List<FootballerData>();
                _unitOperationalDataByTeam[TeamKey.Blue] = new List<FootballerData>();
                
                foreach (var footballer in _unitsProvider.Footballers)
                {
                    var data = new FootballerData(footballer);
                    _unitOperationalDataByTeam[footballer.Team].Add(data);
                }
            }

            var ballPosition = _ballPositionProvider.Position;
            foreach (var unitData in _unitOperationalDataByTeam[TeamKey.Red])
            {
                UpdateDistanceAndResetFlag(unitData, ballPosition);
            }
            
            foreach (var unitData in _unitOperationalDataByTeam[TeamKey.Blue])
            {
                UpdateDistanceAndResetFlag(unitData, ballPosition);
            }
        }

        private static void UpdateDistanceAndResetFlag(FootballerData unitData, Vector3 ballPosition)
        {
            unitData.DistanceToBall = Vector3.Distance(unitData.FootballerUnit.Position, ballPosition);
            unitData.IsUnitRoleDefined = unitData.FootballerUnit.Role == FootballerRole.Goalkeeper;
        }

        private class FootballerData
        {
            public readonly IFootballerUnit FootballerUnit;
            
            public bool IsUnitRoleDefined;
            public float DistanceToBall;

            public FootballerData(IFootballerUnit unit)
            {
                FootballerUnit = unit;
            }
        }
    }
}