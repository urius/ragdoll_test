using Src.Model;
using Src.Providers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Src.Factories
{
    public class FootballerUnitFactory : IFootballerUnitFactory
    {
        private readonly IPrefabsProvider _prefabsProvider;
        private readonly IGameUnitsHolder _gameUnitsHolder;
        private readonly IObjectResolver _objectResolver;

        public FootballerUnitFactory(
            IPrefabsProvider prefabsProvider,
            IGameUnitsHolder gameUnitsHolder,
            IObjectResolver objectResolver)
        {
            _prefabsProvider = prefabsProvider;
            _gameUnitsHolder = gameUnitsHolder;
            _objectResolver = objectResolver;
        }
        
        public IFootballerUnit Create(Vector3 position)
        {
            var unit = _objectResolver.Instantiate(_prefabsProvider.FootballerUnit, position, Quaternion.identity);
            _gameUnitsHolder.AddFootballer(unit);

            return unit;
        }
    }

    public interface IFootballerUnitFactory
    {
        public IFootballerUnit Create(Vector3 position);
    }
}