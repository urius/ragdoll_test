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
        private readonly IFootballerUnitFactory _unitFactory;
        private readonly IStartPointsProvider _startPointsProvider;
        private readonly PlayerControlledUnitProvider _playerControlledUnitProvider;
        private readonly IGameUnitsProvider _unitsProvider;
        private readonly ICameraDirectionProvider _cameraDirectionProvider;

        public FootballersController(
            IFootballerUnitFactory unitFactory,
            IStartPointsProvider startPointsProvider,
            PlayerControlledUnitProvider playerControlledUnitProvider,
            IGameUnitsProvider unitsProvider,
            ICameraDirectionProvider cameraDirectionProvider)
        {
            _unitFactory = unitFactory;
            _startPointsProvider = startPointsProvider;
            _playerControlledUnitProvider = playerControlledUnitProvider;
            _unitsProvider = unitsProvider;
            _cameraDirectionProvider = cameraDirectionProvider;
        }
        
        public void Start()
        {
            Debug.Log("FootballersPresenter.Start");

            CreateFootballers();
        }

        private void CreateFootballers()
        {
            for (var i = 0; i < 3; i++)
            {
                var unit = CreateFootballerUnit(TeamKey.Red, i);
                if(i == 0) _playerControlledUnitProvider.SetTargetUnit(unit);
                
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