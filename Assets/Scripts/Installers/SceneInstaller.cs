using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TouchField>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IMovable>().FromComponentInHierarchy().AsCached();
        Container.Bind<IBattleMovable>().FromComponentInHierarchy().AsCached();
        
        Container.Bind<BattlePlatform>().FromComponentsInHierarchy().AsSingle();
    }
}