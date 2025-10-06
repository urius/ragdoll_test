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
                    else if(Random.value < 0.1f)
                    {
                        UpdateMoveToSupportPointState(footballer);
                    }
                    break;
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

                var supportPosition = attackerPosition + forward * 50;

                var x = attackerPosition.x > 0
                    ? _fieldBorderPositionProvider.GetLeftQuarterX()
                    : _fieldBorderPositionProvider.GetRightQuarterX();

                supportPosition.x = x;

                supportPosition = _fieldBorderPositionProvider.ClampByBorder(supportPosition, 10);

                footballer.SetMoveToTargetPointState(supportPosition);
            }
        }
    }
}