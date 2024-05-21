using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();
        Container.Bind<StackManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ParticleManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SoundManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LoadingManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerAnimationManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CollectableManager>().FromComponentInHierarchy().AsSingle();
    }
}
