using System;
using Src.Components;
using Src.Controllers.RolesBehaviourProcessors;
using Src.Data;
using Src.Factories;
using Src.Model;
using Src.Providers;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Src.Controllers.FootballersController
{
    public class FootballersController : IStartable, IFixedTickable, ITickable
    {
        private const int LogicUpdateFixedTicksCount = 15;
        
        private readonly IFootballerUnitFactory _unitFactory;
        private readonly IStartPointsProvider _startPointsProvider;
        private readonly PlayerControlledUnitProvider _playerControlledUnitProvider;
        private readonly IGameUnitsProvider _unitsProvider;
        private readonly ICameraDirectionProvider _cameraDirectionProvider;
        private readonly GoalkeeperBehaviourProcessor _goalkeeperBehaviourProcessor;
        private readonly AttackerBehaviourProcessor _attackerBehaviourProcessor;
        private readonly AttackerSupportBehaviourProcessor _attackerSupportBehaviourProcessor;
        private readonly DefenderBehaviourProcessor _defenderBehaviourProcessor;
        private readonly UndefinedRoleBehaviourProcessor _undefinedRoleBehaviour;
        private readonly DefineRolesLogic _defineRolesLogic;

        private int _fixedTicksCounter = 0;

        public FootballersController(
            IFootballerUnitFactory unitFactory,
            IStartPointsProvider startPointsProvider,
            PlayerControlledUnitProvider playerControlledUnitProvider,
            IGameUnitsProvider unitsProvider,
            ICameraDirectionProvider cameraDirectionProvider,
            GoalkeeperBehaviourProcessor goalkeeperBehaviourProcessor,
            AttackerBehaviourProcessor attackerBehaviourProcessor,
            AttackerSupportBehaviourProcessor attackerSupportBehaviourProcessor,
            DefenderBehaviourProcessor defenderBehaviourProcessor,
            UndefinedRoleBehaviourProcessor undefinedRoleBehaviour,
            DefineRolesLogic defineRolesLogic)
        {
            _unitFactory = unitFactory;
            _startPointsProvider = startPointsProvider;
            _playerControlledUnitProvider = playerControlledUnitProvider;
            _unitsProvider = unitsProvider;
            _cameraDirectionProvider = cameraDirectionProvider;
            _goalkeeperBehaviourProcessor = goalkeeperBehaviourProcessor;
            _attackerBehaviourProcessor = attackerBehaviourProcessor;
            _attackerSupportBehaviourProcessor = attackerSupportBehaviourProcessor;
            _defenderBehaviourProcessor = defenderBehaviourProcessor;
            _undefinedRoleBehaviour = undefinedRoleBehaviour;
            _defineRolesLogic = defineRolesLogic;
        }
        
        public void Start()
        {
            CreateFootballers();
            
            _defineRolesLogic.DefineGoalkeepers();
            _defineRolesLogic.UpdateRoles();
        }

        public void FixedTick()
        {
            _fixedTicksCounter++;

            if (_fixedTicksCounter > LogicUpdateFixedTicksCount)
            {
                _fixedTicksCounter = 0;
                
                _defineRolesLogic.UpdateRoles();
                ProcessFootballersBehaviourLogic();
            }
        }

        public void Tick()
        {
            ProcessPlayerControlledUnit();
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
                if (_playerControlledUnitProvider.TargetUnit == footballer)
                {
                    _playerControlledUnitProvider.TargetUnit.SetPlayerControlledBehaviourState();
                    continue;
                }
                
                GetRoleBehaviourProcessor(footballer.Role).Process(footballer);
            }
        }

        private IRoleBehaviourProcessor GetRoleBehaviourProcessor(FootballerRole role)
        {
            switch (role)
            {
                case FootballerRole.Goalkeeper:
                    return _goalkeeperBehaviourProcessor;
                case FootballerRole.Attacker:
                    return _attackerBehaviourProcessor;
                case FootballerRole.AttackerSupport:
                    return _attackerSupportBehaviourProcessor;
                case FootballerRole.Defender:
                    return _defenderBehaviourProcessor;
                case FootballerRole.Undefined:
                    return _undefinedRoleBehaviour;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
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
                unit.SetHittingBallState(_cameraDirectionProvider.Forward, 50, 7);

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

            if (Keyboard.current.leftShiftKey.isPressed)
            {
                _playerControlledUnitProvider.TargetUnit.SetMaxSpeed(25);
            }
            else
            {
                _playerControlledUnitProvider.TargetUnit.SetMaxSpeed(15);
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