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
        
        public IFootballerUnit Create(Vector3 position, Quaternion rotation)
        {
            var unit = _objectResolver.Instantiate(_prefabsProvider.FootballerUnit);
            
            var transform = unit.transform;
            position.y = transform.position.y;
            transform.position = position;
            transform.rotation = rotation;
            
            _gameUnitsHolder.AddFootballer(unit);

            return unit;
        }
    }

    public interface IFootballerUnitFactory
    {
        public IFootballerUnit Create(Vector3 position, Quaternion quaternion);
    }
}