using System.Collections.Generic;
using System.Linq;
using Src.Data;
using Src.Data.BehaviourStates;
using Src.Model;
using Src.Providers;
using UnityEngine;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public class AttackerSupportBehaviourProcessor : IRoleBehaviourProcessor
    {
        private readonly IGameUnitsProvider _unitsProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;
        private readonly IFieldBorderPositionProvider _fieldBorderPositionProvider;

        private readonly List<IFootballerUnit> _attackerSupportUnitsList = new(3);

        public AttackerSupportBehaviourProcessor(
            IGameUnitsProvider unitsProvider,
            IGoalGatesProvider goalGatesProvider,
            IFieldBorderPositionProvider fieldBorderPositionProvider)
        {
            _unitsProvider = unitsProvider;
            _goalGatesProvider = goalGatesProvider;
            _fieldBorderPositionProvider = fieldBorderPositionProvider;
        }

        public void Process(IFootballerUnit footballer)
        {
            UpdateAttackSupportersList(footballer);
            
            switch (footballer.BehaviourState)
            {
                case BehaviourStateName.Undefined:
                    UpdateMoveToSupportPointState(footballer);
                    break;
                case BehaviourStateName.MoveToPoint:
                    if (footballer.IsOnTargetPoint())
                    {
                        footballer.ResetBehaviourState();
                    }
                    else if (Random.value < 0.1f)
                    {
                        UpdateMoveToSupportPointState(footballer);
                    }

                    break;
            }
        }

        private void UpdateAttackSupportersList(IFootballerUnit footballer)
        {
            var shouldInsert = true;
            
            for (var i = 0; i < _attackerSupportUnitsList.Count; i++)
            {
                var tempFootballer = _attackerSupportUnitsList[i];
                if (tempFootballer == footballer)
                {
                    shouldInsert = false;
                }
                else if (_attackerSupportUnitsList[i].Role != FootballerRole.AttackerSupport)
                {
                    _attackerSupportUnitsList.Remove(tempFootballer);
                    i--;
                }
            }

            if (shouldInsert)
            {
                _attackerSupportUnitsList.Add(footballer);
            }
        }

        private void UpdateMoveToSupportPointState(IFootballerUnit footballer)
        {
            var activeAttacker = _unitsProvider.Footballers.FirstOrDefault(f =>
                f.Team == footballer.Team
                && f.Role == FootballerRole.Attacker);

            if (activeAttacker != null)
            {
                var forward = _goalGatesProvider.GetGatesForTeam(footballer.Team).Forward;
                var attackerPosition = activeAttacker.PositionProjected;

                var targetPosition = IndexOfTeamUnit(footballer) == 0
                    ? GetSupportPositionSide(attackerPosition, forward)
                    : GetSupportPositionMiddle(attackerPosition, forward);
                
                footballer.SetMoveToTargetPointState(targetPosition);
            }
        }

        private Vector3 GetSupportPositionSide(Vector3 attackerPosition, Vector3 forward)
        {
            var supportPositionSide = attackerPosition + forward * 50;
            var x = attackerPosition.x > 0
                ? _fieldBorderPositionProvider.GetLeftQuarterX()
                : _fieldBorderPositionProvider.GetRightQuarterX();
            supportPositionSide.x = x;
            supportPositionSide = _fieldBorderPositionProvider.ClampByBorder(supportPositionSide, 20);
            return supportPositionSide;
        }

        private Vector3 GetSupportPositionMiddle(Vector3 attackerPosition, Vector3 forward)
        {
            var supportPositionMiddle = attackerPosition + forward * 20;
            supportPositionMiddle.x = 0;
            supportPositionMiddle = _fieldBorderPositionProvider.ClampByBorder(supportPositionMiddle, 30);
            return supportPositionMiddle;
        }

        private int IndexOfTeamUnit(IFootballerUnit footballer)
        {
            var index = -1;
            foreach (var unit in _attackerSupportUnitsList)
            {
                if (unit.Team == footballer.Team)
                {
                    index++;
                    if (unit == footballer)
                    {
                        break;
                    }
                }
            }

            return index;
        }
    }
}