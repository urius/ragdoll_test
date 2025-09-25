using Src.Components;
using Src.Controllers;
using Src.Factories;
using Src.Presenters;
using Src.Providers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Src.Infra
{
    public class GameRoundLifetimeScope : LifetimeScope
    {
        [SerializeField] private StartPointsProvider _startPointsProvider;
        [SerializeField] private PrefabsProvider _prefabsProvider;
        [SerializeField] private CameraPresenter _cameraPresenter;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<IStartPointsProvider>(_startPointsProvider);
            builder.RegisterInstance<IPrefabsProvider>(_prefabsProvider);
            builder.RegisterInstance(_cameraPresenter).AsImplementedInterfaces();
            
            builder.Register<GameUnitsProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<FootballerUnitFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlayerControlledUnitProvider>(Lifetime.Singleton);

            builder.RegisterEntryPoint<FootballersController>();
        }
    }
}