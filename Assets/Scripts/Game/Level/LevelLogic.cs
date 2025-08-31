using UnityEngine;
using Zenject;

public class LevelLogic : MonoBehaviour
{
    private SceneLoader _sceneLoader;
    private PlayerMovement _playerMovement;

    [Inject]
    private void Construct(SceneLoader sceneLoader, PlayerMovement playerMovement)
    {
        _sceneLoader = sceneLoader;
        _playerMovement = playerMovement;
    }

    private void OnEnable()
    {
        _playerMovement.OnPlayerHitEnemy += LevelLosing;
    }
    
    private void OnDisable()
    {
        _playerMovement.OnPlayerHitEnemy -= LevelLosing;
    }
    
    private void LevelLosing()
    {
        _sceneLoader.RestartSceneWithDelay(0f);
    }
}
