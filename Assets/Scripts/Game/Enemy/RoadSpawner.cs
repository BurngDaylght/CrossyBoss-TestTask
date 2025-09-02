using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private RoadEnemy[] _enemyPrefabs;
    [SerializeField] private int _initialEnemies = 5;
    [SerializeField] private Vector3 _spawnOffset;
    [SerializeField] private float _xSpawnStart = -10f;
    [SerializeField] private float _xSpawnEnd = 10f;

    [Header("Spawn Timing")]
    [SerializeField] private float _minSpawnInterval = 1f;
    [SerializeField] private float _maxSpawnInterval = 3f;

    [Header("Enemy Speed")]
    [SerializeField] private float _minSpeed = 1f;
    [SerializeField] private float _maxSpeed = 3f;

    [Header("Pool Settings")]
    [SerializeField] private Transform _poolParent;
    [SerializeField] private int _prewarmObjectsCount = 10;

    private List<CustomPool<RoadEnemy>> _pools;
    private Vector3 _spawnDirection;
    private float _randomSpeed;

    private void Start()
    {
        _pools = new List<CustomPool<RoadEnemy>>(_enemyPrefabs.Length);
        for (int i = 0; i < _enemyPrefabs.Length; i++)
        {
            var poolParent = _poolParent != null ? _poolParent : transform;
            _pools.Add(new CustomPool<RoadEnemy>(_enemyPrefabs[i], _prewarmObjectsCount, poolParent));
        }

        _randomSpeed = UnityEngine.Random.Range(_minSpeed, _maxSpeed);

        _spawnDirection = UnityEngine.Random.value > 0.5f ? Vector3.right : Vector3.left;

        float spacing = (_xSpawnEnd - _xSpawnStart) / Mathf.Max(1, _initialEnemies);
        for (int i = 0; i < _initialEnemies; i++)
        {
            float offset = UnityEngine.Random.Range(0f, spacing * 0.5f);
            float xPos = _xSpawnStart + i * spacing + offset;
            SpawnEnemy(xPos);
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float delay = UnityEngine.Random.Range(_minSpawnInterval, _maxSpawnInterval);
            yield return new WaitForSeconds(delay);

            float spawnX = _spawnDirection == Vector3.right ? _xSpawnStart : _xSpawnEnd;
            SpawnEnemy(spawnX);
        }
    }

    private void SpawnEnemy(float xPosition)
    {
        int prefabIndex = UnityEngine.Random.Range(0, _pools.Count);
        SpawnEnemy(xPosition, prefabIndex);
    }

    private void SpawnEnemy(float xPosition, int prefabIndex)
    {
        if (_pools == null || prefabIndex < 0 || prefabIndex >= _pools.Count) return;

        RoadEnemy enemy = _pools[prefabIndex].Get();
        if (enemy == null) return;

        Vector3 spawnPos = new Vector3(
            xPosition + _spawnOffset.x,
            transform.position.y + _spawnOffset.y,
            transform.position.z + _spawnOffset.z
        );

        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;

        enemy.SetMoveDirection(_spawnDirection);
        enemy.SetSpeed(_randomSpeed);
        enemy.SetLimit(Math.Abs(_xSpawnStart));
        enemy.SetPool(_pools[prefabIndex]);
    }
}
