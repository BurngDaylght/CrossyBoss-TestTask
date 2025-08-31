using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ProjectContext", menuName = "Installers/ProjectContext")]
public class ProjectContext : ScriptableObjectInstaller<ProjectContext>
{
    public override void InstallBindings()
    {
        Container.Bind<SceneLoader>().FromComponentInHierarchy().AsSingle();
    }
}