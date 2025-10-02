using System.Collections.Generic;
using Src.Components;
using Src.Controllers.RolesBehaviourProcessors;
using Src.Data;
using Src.Factories;
using Src.Model;
using Src.Providers;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Src.Controllers
{
    public class FootballersController : IStartable, IFixedTickable, ITickable
    {
        private const int LogicUpdateFixedTicksCount = 15;
        
        private readonly IFootballerUnitFactory _unitFactory;
        private readonly IStartPointsProvider _startPointsProvider;
        private readonly PlayerControlledUnitProvider _playerControlledUnitProvider;
        private readonly IGameUnitsProvider _unitsProvider;
        private readonly ICameraDirectionProvider _cameraDirectionProvider;
        private readonly IGoalGatesProvider _goalGatesProvider;
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly GoalkeeperBehaviourProcessor _goalkeeperBehaviourProcessor;
        private readonly AttackerBehaviourProcessor _attackerBehaviourProcessor;
        private readonly DefenderBehaviourProcessor _defenderBehaviourProcessor;

        private int _fixedTicksCounter = 0;

        public FootballersController(
            IFootballerUnitFactory unitFactory,
            IStartPointsProvider startPointsProvider,
            PlayerControlledUnitProvider playerControlledUnitProvider,
            IGameUnitsProvider unitsProvider,
            ICameraDirectionProvider cameraDirectionProvider,
            IGoalGatesProvider goalGatesProvider,
            IBallPositionProvider ballPositionProvider,
            GoalkeeperBehaviourProcessor goalkeeperBehaviourProcessor,
            AttackerBehaviourProcessor attackerBehaviourProcessor,
            DefenderBehaviourProcessor defenderBehaviourProcessor)
        {
            _unitFactory = unitFactory;
            _startPointsProvider = startPointsProvider;
            _playerControlledUnitProvider = playerControlledUnitProvider;
            _unitsProvider = unitsProvider;
            _cameraDirectionProvider = cameraDirectionProvider;
            _goalGatesProvider = goalGatesProvider;
            _ballPositionProvider = ballPositionProvider;
            _goalkeeperBehaviourProcessor = goalkeeperBehaviourProcessor;
            _attackerBehaviourProcessor = attackerBehaviourProcessor;
            _defenderBehaviourProcessor = defenderBehaviourProcessor;
        }
        
        public void Start()
        {
            Debug.Log("FootballersPresenter.Start");

            CreateFootballers();
            DefineGoalkeepers();
            UpdateRoles();
        }

        public void FixedTick()
        {
            _fixedTicksCounter++;

            if (_fixedTicksCounter > LogicUpdateFixedTicksCount)
            {
                _fixedTicksCounter = 0;
                UpdateRoles();
                ProcessFootballersBehaviourLogic();
            }
        }

        public void Tick()
        {
            ProcessPlayerControlledUnit();
        }

        private void DefineGoalkeepers()
        {
            var closestToGoalGatesUnits = new Dictionary<TeamKey, IFootballerUnit>(2);
            
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
                kvp.Value.Role = FootballerRole.Goalkeeper;
            }
        }

        private void UpdateRoles()
        {
            var closestToBallUnits = new Dictionary<TeamKey, IFootballerUnit>(2);
            var ballPosition = _ballPositionProvider.Position;
            
            foreach (var footballerUnit in _unitsProvider.Footballers)
            {
                if (footballerUnit.Role == FootballerRole.Goalkeeper) continue;

                footballerUnit.Role = FootballerRole.Defender;
                
                var team = footballerUnit.Team;
                if (closestToBallUnits.ContainsKey(team) == false)
                {
                    closestToBallUnits[team] = footballerUnit;
                    continue;
                }

                if (Vector3.Distance(ballPosition, footballerUnit.Position) <
                    Vector3.Distance(ballPosition, closestToBallUnits[team].Position))
                {
                    closestToBallUnits[team] = footballerUnit;
                }
            }
            
            foreach (var closestToBallUnit in closestToBallUnits.Values)
            {
                closestToBallUnit.Role = FootballerRole.Attacker;
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

        private void ProcessFootballersBehaviourLogic()
        {
            foreach (var footballer in _unitsProvider.Footballers)
            {
                if (_playerControlledUnitProvider.TargetUnit == footballer) continue;
                
                switch (footballer.Role)
                {
                    case FootballerRole.Goalkeeper:
                        _goalkeeperBehaviourProcessor.Process(footballer);
                        break;
                    case FootballerRole.Attacker:
                        _attackerBehaviourProcessor.Process(footballer);
                        break;
                    case FootballerRole.Defender:
                        _defenderBehaviourProcessor.Process(footballer);
                        break;
                }
            }
        }

        private void ProcessPlayerControlledUnit()
        {
            if (_playerControlledUnitProvider.TargetUnit == null) return;
            
            var unit = _playerControlledUnitProvider.TargetUnit;

            const float deltaTime = 0.005f;
            const float defaultFixedDeltaTime = 0.02f;
            
            if (Keyboard.current.spaceKey.isPressed)
            {
                var hitDirection = _cameraDirectionProvider.Forward * 50;
                hitDirection.y = 7;
                SetHittingBallState(unit, hitDirection);

                Debug.Log("Time.fixedDeltaTime: " + Time.fixedDeltaTime);
                
                var newTimeScale = Mathf.Max(0.01f, Time.timeScale - deltaTime);
                if (Time.timeScale >= 1)
                {
                    newTimeScale = 0.2f;
                }
                Time.timeScale = newTimeScale;
                Time.fixedDeltaTime = Time.timeScale * defaultFixedDeltaTime;
                
                return;
            }

            if (Time.timeScale < 1)
            {
                Time.timeScale = Mathf.Min(1, Time.timeScale + deltaTime);
                Time.fixedDeltaTime = Time.timeScale * defaultFixedDeltaTime;
            }

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

        private void SetHittingBallState(IFootballerUnit unit, Vector3 hitDirection)
        {
            var ballVector = _ballPositionProvider.PositionProjected - unit.PositionProjected;
            var ballVectorAngle = Vector3.SignedAngle(unit.ForwardProjected, ballVector, Vector3.up);
            if (ballVectorAngle > 0)
            {
                unit.SetHittingBallStateRightLeg(hitDirection);
            }
            else
            {
                unit.SetHittingBallStateLeftLeg(hitDirection);
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