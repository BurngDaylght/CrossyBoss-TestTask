using System;
using System.Collections;
using UnityEngine;

public class BattleSpawner : MonoBehaviour
{
    public event Action<BattleEnemy> OnEnemySpawned;

    [Header("Enemy Settings")]
    [SerializeField] private BattleEnemy _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnDelay = 0.5f;
    [SerializeField] private Vector3 _spawnOffset;

    [Header("Pool Settings")]
    [SerializeField] private Transform _poolParent;
    [SerializeField] private int _prewarmObjectsCount = 5;

    private CustomPool<BattleEnemy> _battleEnemyPool;
    private bool _spawned = false;

    private void Awake()
    {
        _battleEnemyPool = new CustomPool<BattleEnemy>(_enemyPrefab, _prewarmObjectsCount, _poolParent);
    }

    public void SpawnEnemies(Transform player)
    {
        if (_spawned) return;
        _spawned = true;

        StartCoroutine(SpawnCoroutine(player));
    }

    private IEnumerator SpawnCoroutine(Transform player)
    {
        foreach (var point in _spawnPoints)
        {
            BattleEnemy enemy = _battleEnemyPool.Get();

            Vector3 spawnPos = point.position + _spawnOffset;
            enemy.transform.position = spawnPos;

            enemy.transform.rotation = Quaternion.identity;
            enemy.SetPlayerTarget(player);
            enemy.SetPool(_battleEnemyPool);
            enemy.gameObject.SetActive(true);

            OnEnemySpawned?.Invoke(enemy);

            yield return new WaitForSeconds(_spawnDelay);
        }
    }
}
