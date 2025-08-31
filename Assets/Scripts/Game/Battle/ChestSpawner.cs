using UnityEngine;
using Zenject;

public class ChestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _chestPrefab;
    [SerializeField] private Transform _spawnPoint;

    private PlayerMovement _playerMovement;
    
    [Inject]
    private void Construct(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
    }

    private void OnEnable()
    {
        BattlePlatform.OnAllEnemiesDefeated += SpawnChest;
    }

    private void OnDisable()
    {
        BattlePlatform.OnAllEnemiesDefeated -= SpawnChest;
    }

    public void SpawnChest()
    {
        Instantiate(_chestPrefab, _spawnPoint.position, Quaternion.identity, _spawnPoint);
    }
}
