using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BattlePlatform : MonoBehaviour
{
    [Header("Battle Settings")]
    [SerializeField, Min(0.1f)] private float _zoneSize = 1f;
    [SerializeField] private Vector3 _zoneCenter = Vector3.zero;
    [SerializeField] private bool _useTrigger = true;
    [SerializeField, Min(0f)] private float _spawnDelay = 1f;

    public float ZoneSize => _zoneSize;
    public Vector3 ZoneCenter => _zoneCenter;
    public IReadOnlyList<BattleEnemy> ActiveEnemies => _activeEnemies;

    public event Action OnPlayerEnterBattleZone;
    public event Action OnPlayerExitBattleZone;
    public event Action OnAllEnemiesDefeated;

    private List<BattleEnemy> _activeEnemies = new List<BattleEnemy>();
    private Transform _player;
    private BattleSpawner _battleSpawner;
    private SphereCollider _collider;
    
    private void OnValidate()
    {
        _collider = GetComponent<SphereCollider>();
        ConfigureCollider();
    }
    
    private void OnEnable()
    {
        _battleSpawner.OnEnemySpawned += RegisterEnemy;
    }

    private void OnDisable()
    {
        _battleSpawner.OnEnemySpawned -= RegisterEnemy;
    }
    
    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        ConfigureCollider();

        _battleSpawner = GetComponentInChildren<BattleSpawner>();
    }

    private void ConfigureCollider()
    {
        _collider.isTrigger = _useTrigger;
        _collider.radius = _zoneSize;
        _collider.center = _zoneCenter;
    }

    public Vector3 GetWorldCenter() => transform.TransformPoint(_zoneCenter);

    public float GetWorldRadius()
    {
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        return _zoneSize * maxScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            _player = player.transform;

            var playerBattle = _player.GetComponent<PlayerBattle>();
            playerBattle?.SetBattlerPlatform(this);

            OnPlayerEnterBattleZone?.Invoke();

            StartCoroutine(SpawnEnemiesWithDelay());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerStats>(out _))
            OnPlayerExitBattleZone?.Invoke();
    }

    private IEnumerator SpawnEnemiesWithDelay()
    {
        yield return new WaitForSeconds(_spawnDelay);
        _battleSpawner?.SpawnEnemies(_player);
    }

    private void RegisterEnemy(BattleEnemy enemy)
    {
        if (!_activeEnemies.Contains(enemy))
            _activeEnemies.Add(enemy);

        enemy.OnEnemyDied += () =>
        {
            _activeEnemies.Remove(enemy);
            if (_activeEnemies.Count == 0)
                OnAllEnemiesDefeated?.Invoke();
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.TransformPoint(_zoneCenter), GetWorldRadius());
    }
}
