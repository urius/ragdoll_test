using Src.Components;
using Src.Controllers;
using Src.Controllers.RolesBehaviourProcessors;
using Src.Factories;
using Src.Presenters;
using Src.Providers;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Src.Infra
{
    public class GameRoundLifetimeScope : LifetimeScope
    {
        [SerializeField] private StartPointsProvider _startPointsProvider;
        [SerializeField] private PrefabsProvider _prefabsProvider;
        [FormerlySerializedAs("_cameraPresenter")] [SerializeField] private CameraContainerPresenter _cameraContainerPresenter;
        [SerializeField] private BallFacade _ballFacade;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<IStartPointsProvider>(_startPointsProvider);
            builder.RegisterInstance<IPrefabsProvider>(_prefabsProvider);
            builder.RegisterInstance(_cameraContainerPresenter).AsImplementedInterfaces();
            builder.RegisterInstance(_ballFacade).AsImplementedInterfaces();
            
            builder.Register<GameUnitsProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<FootballerUnitFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlayerControlledUnitProvider>(Lifetime.Singleton);
            builder.Register<GoalkeeperBehaviourProcessor>(Lifetime.Singleton);
            builder.Register<AttackerBehaviourProcessor>(Lifetime.Singleton);
            builder.Register<DefenderBehaviourProcessor>(Lifetime.Singleton);

            builder.RegisterEntryPoint<GoalGatesProvider>();
            builder.RegisterEntryPoint<GameController>();
            builder.RegisterEntryPoint<FootballersController>();
        }
    }
}