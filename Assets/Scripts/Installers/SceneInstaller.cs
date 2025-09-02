using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindPlayer();
        BindChest();
        BindLevel();
        BindInput();
        BindMovement();
        BindBattle();
        BindUI();
    }

    private void BindPlayer()
    {
        Container.Bind<PlayerRoadMovement>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerAnimation>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerStats>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerBattle>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerWeapon>().FromComponentInHierarchy().AsSingle();
    }

    private void BindChest()
    {
        Container.Bind<ChestLock>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Chest>().FromComponentInHierarchy().AsSingle();
    }

    private void BindLevel()
    {
        Container.Bind<LevelLogic>().FromComponentInHierarchy().AsSingle();
    }

    private void BindInput()
    {
        Container.Bind<TouchField>().FromComponentInHierarchy().AsSingle();
        Container.Bind<InputHandler>().FromComponentInHierarchy().AsSingle();
    }

    private void BindMovement()
    {
        Container.Bind<IRoadMovable>().FromComponentInHierarchy().AsCached();
        Container.Bind<IBattleMovable>().FromComponentInHierarchy().AsCached();
    }

    private void BindBattle()
    {
        Container.Bind<BattlePlatform>().FromComponentsInHierarchy().AsSingle();
    }
    
    private void BindUI()
    {
        Container.Bind<StickUI>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ChestLockUI>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<CompleteLevelUI>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<BattleStartTextUI>().FromComponentsInHierarchy().AsSingle();
    }
}
