using System.Collections.Generic;
using System.Linq;
using Src.Components;
using Src.Data;
using Src.Factories;
using Src.Model;
using Src.Providers;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Src.Controllers
{
    public class FootballersController : IStartable, IFixedTickable
    {
        private const int LogicUpdateFixedTicksCount = 15;
        
        private readonly IFootballerUnitFactory _unitFactory;
        private readonly IStartPointsProvider _startPointsProvider;
        private readonly PlayerControlledUnitProvider _playerControlledUnitProvider;
        private readonly IGameUnitsProvider _unitsProvider;
        private readonly ICameraDirectionProvider _cameraDirectionProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;
        private readonly IBallPositionProvider _ballPositionProvider;

        private int _fixedTicksCounter = 0;

        public FootballersController(
            IFootballerUnitFactory unitFactory,
            IStartPointsProvider startPointsProvider,
            PlayerControlledUnitProvider playerControlledUnitProvider,
            IGameUnitsProvider unitsProvider,
            ICameraDirectionProvider cameraDirectionProvider,
            IGoalGatesProvider goalGatesProvider,
            IBallPositionProvider ballPositionProvider)
        {
            _unitFactory = unitFactory;
            _startPointsProvider = startPointsProvider;
            _playerControlledUnitProvider = playerControlledUnitProvider;
            _unitsProvider = unitsProvider;
            _cameraDirectionProvider = cameraDirectionProvider;
            _goalGatesProvider = goalGatesProvider;
            _ballPositionProvider = ballPositionProvider;
        }
        
        public void Start()
        {
            Debug.Log("FootballersPresenter.Start");

            CreateFootballers();
            DefineGoalkeepers();
        }

        private void DefineGoalkeepers()
        {
            var closestToGoalGatesUnits = new Dictionary<TeamKey, IFootballerUnit>(_unitsProvider.Footballers.Count());
            
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
                kvp.Value.BehaviourStrategy = FootballerBehaviourStrategy.Goalkeeper;
            }
        }

        private void CreateFootballers()
        {
            for (var i = 0; i < _startPointsProvider.PointsAmount; i++)
            {
                var unit = CreateFootballerUnit(TeamKey.Red, i);
                if (i == 0) _playerControlledUnitProvider.SetTargetUnit(unit);
                
                CreateFootballerUnit(TeamKey.Blue, i);
            }
        }

        private IFootballerUnit CreateFootballerUnit(TeamKey team, int innerTeamIndex)
        {
            var position = _startPointsProvider.GetPointPosition(innerTeamIndex, team);
            var targetDirection = new Vector3(0, 0, -position.z);
            var rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            var unit = _unitFactory.Create(position, rotation);
            unit.SetupData(team, 0);
            unit.SetTargetDirection(targetDirection);
            
            return unit;
        }

        public void FixedTick()
        {
            ProcessPlayerControlledUnit();

            _fixedTicksCounter++;

            if (_fixedTicksCounter > LogicUpdateFixedTicksCount)
            {
                _fixedTicksCounter = 0;
                ProcessFootballersBehaviourLogic();
            }
        }

        private void ProcessFootballersBehaviourLogic()
        {
            ProcessGoalKeepersLogic();
        }

        private void ProcessGoalKeepersLogic()
        {
            var ballPosition = _ballPositionProvider.Position;
            
            foreach (var footballer in _unitsProvider.Footballers)
            {
                if (footballer.BehaviourStrategy == FootballerBehaviourStrategy.Goalkeeper)
                {
                    var gates = _goalGatesProvider.GetGatesForTeam(footballer.Team);
                    var gateBounds = gates.BoundPositions;
                    var gateMaxXBounds = gateBounds.Max(v => v.x);
                    var gateMinXBounds = gateBounds.Min(v => v.x);
                    var targetGoalKeeperXPosition = Mathf.Clamp(ballPosition.x, gateMinXBounds, gateMaxXBounds);

                    var targetPos = new Vector3(targetGoalKeeperXPosition, 0, gateBounds[0].z);
                    footballer.SetMovingToTargetPointState(targetPos);
                    if (footballer.IsOnTargetPoint())
                    {
                        footballer.SetTargetDirection(gates.Forward);
                    }
                }
            }
        }

        private void ProcessPlayerControlledUnit()
        {
            if (_playerControlledUnitProvider.TargetUnit == null) return;
            
            var unit = _playerControlledUnitProvider.TargetUnit;
            var directionVectorLocal = GetDirectionLocalVectorByKeyboard();

            var directionVector = _cameraDirectionProvider.Forward * directionVectorLocal.z +
                                  _cameraDirectionProvider.Right * directionVectorLocal.x;

            if (directionVectorLocal != Vector3.zero)
            {
                unit.SetTargetDirection(directionVector);
                unit.SetMovingState();
            }
            else
            {
                unit.SetStandingState();
            }
        }

        private Vector3 GetDirectionLocalVectorByKeyboard()
        {
            var result = Vector3.zero;
            
            var keyboard = Keyboard.current;
            if (keyboard.wKey.isPressed)
            {
                result.z += 1;
            }
            if (keyboard.sKey.isPressed)
            {
                result.z -= 1;
            }
            if (keyboard.dKey.isPressed)
            {
                result.x += 1;
            }
            if (keyboard.aKey.isPressed)
            {
                result.x-= 1;
            }

            return result;
        }
    }
}