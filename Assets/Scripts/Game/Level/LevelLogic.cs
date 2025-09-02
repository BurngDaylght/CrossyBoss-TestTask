using System;
using UnityEngine;
using Zenject;

public class LevelLogic : MonoBehaviour
{
    public event Action OnLevelStart;
    public event Action OnLevelLosing;
    public event Action OnLevelComplete;
    public event Action OnLevelRestart;
    public event Action OnFirstTap;

    private bool _firstTapped = false;
    private bool _isRestarting = false;

    private SceneLoader _sceneLoader;
    private PlayerRoadMovement _playerMovement;
    private PlayerStats _playerStats;
    private InputHandler _inputHandler;
    private ChestLock _chestLock;

    [Inject]
    private void Construct(SceneLoader sceneLoader, PlayerRoadMovement playerMovement, InputHandler inputHandler, PlayerStats playerStats, ChestLock chestLock)
    {
        _sceneLoader = sceneLoader;
        _playerMovement = playerMovement;
        _inputHandler = inputHandler;
        _playerStats = playerStats;
        _chestLock = chestLock;
    }

    private void OnEnable()
    {
        _playerMovement.OnPlayerHitEnemy += LoseLevel;
        _playerStats.OnPlayerDied += LoseLevel;
        _inputHandler.OnTap += HandleTap;
        
        _chestLock.OnLockCompleted += CompleteLevel;
    }

    private void OnDisable()
    {
        _playerMovement.OnPlayerHitEnemy -= LoseLevel;
        _playerStats.OnPlayerDied -= LoseLevel;
        _inputHandler.OnTap -= HandleTap;
        
        _chestLock.OnLockCompleted -= CompleteLevel;
    }

    private void HandleTap()
    {
        if (!_firstTapped)
        {
            _firstTapped = true;
            OnFirstTap?.Invoke();
        }
        else
        {
            OnLevelStart?.Invoke();
        }
    }
    
    public void CompleteLevel()
    {
        OnLevelComplete?.Invoke();
    }

    private void LoseLevel()
    {
        if (_isRestarting) return;
        
        OnLevelLosing?.Invoke();
        RestartLevel();
    }
    
    public void RestartLevel()
    {
        if (_isRestarting) return;

        _isRestarting = true;
        OnLevelRestart?.Invoke();
        _sceneLoader.RestartSceneWithDelay(2f);
    }
}
