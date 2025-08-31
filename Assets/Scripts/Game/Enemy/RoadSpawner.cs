using System.Collections;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    [SerializeField] private RoadEnemy _enemyPrefab;
    [SerializeField] private float _minSpawnInterval = 1f;
    [SerializeField] private float _maxSpawnInterval = 3f;
    [SerializeField] private float _xSpawnStart = -10f;
    [SerializeField] private float _xSpawnEnd = 10f;
    [SerializeField] private Vector3 _spawnOffset;
    [SerializeField] private float _minSpeed = 1f;
    [SerializeField] private float _maxSpeed = 3f;
    [SerializeField] private int _initialEnemies = 5;
    
    private Vector3 _spawnDirection;
    private float _randomSpeed;

    private void Start()
    {
        _spawnDirection = Random.value > 0.5f ? Vector3.right : Vector3.left;
        _randomSpeed = Random.Range(_minSpeed, _maxSpeed);

        float spacing = (_xSpawnEnd - _xSpawnStart) / Mathf.Max(1, _initialEnemies);
        for (int i = 0; i < _initialEnemies; i++)
        {
            float offset = Random.Range(0f, spacing * 0.5f);
            float xPos = _xSpawnStart + i * spacing + offset;
            SpawnEnemy(xPos);
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float delay = Random.Range(_minSpawnInterval, _maxSpawnInterval);
            yield return new WaitForSeconds(delay);

            float spawnX = _spawnDirection == Vector3.right ? _xSpawnStart : _xSpawnEnd;
            SpawnEnemy(spawnX);
        }
    }

    private void SpawnEnemy(float xPosition)
    {
        Vector3 spawnPos = new Vector3(
            xPosition + _spawnOffset.x,
            transform.position.y + _spawnOffset.y,
            transform.position.z + _spawnOffset.z
        );

        RoadEnemy enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity, transform);

        enemy.SetMoveDirection(_spawnDirection);
        enemy.SetSpeed(_randomSpeed);
    }
}
