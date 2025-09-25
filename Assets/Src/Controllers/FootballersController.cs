using Src.Components;
using Src.Data;
using Src.Factories;
using Src.Providers;
using UnityEngine;
using VContainer.Unity;

namespace Src.Controllers
{
    public class FootballersController : IStartable, IFixedTickable
    {
        private readonly IFootballerUnitFactory _unitFactory;
        private readonly IStartPointsProvider _startPointsProvider;
        private readonly PlayerControlledUnitProvider _playerControlledUnitProvider;
        private readonly IGameUnitsProvider _unitsProvider;

        public FootballersController(
            IFootballerUnitFactory unitFactory,
            IStartPointsProvider startPointsProvider,
            PlayerControlledUnitProvider playerControlledUnitProvider,
            IGameUnitsProvider unitsProvider)
        {
            _unitFactory = unitFactory;
            _startPointsProvider = startPointsProvider;
            _playerControlledUnitProvider = playerControlledUnitProvider;
            _unitsProvider = unitsProvider;
        }
        
        public void Start()
        {
            Debug.Log("FootballersPresenter.Start");

            //CreateFootballers();
        }

        private void CreateFootballers()
        {
            var team = TeamKey.Red;
            var position = _startPointsProvider.GetPointPosition(0, team);
            
            var unit = _unitFactory.Create(position);
            unit.SetupData(team, 0);
            
            _playerControlledUnitProvider.SetTargetUnit(unit);
        }

        public void FixedTick()
        {
            ProcessPlayerControlledUnit();
        }

        private void ProcessPlayerControlledUnit()
        {
            
        }
    }
}