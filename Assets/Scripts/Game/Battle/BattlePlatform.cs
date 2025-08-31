using System;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlatform : MonoBehaviour
{
    [Header("Battle Settings")]
    [SerializeField] private float _zoneSize = 1f;
    [SerializeField] private Vector3 _zoneCenter = Vector3.zero;
    [SerializeField] private bool _useTrigger = true;
    
    public static event Action OnPlayerEnterBattleZone;
    public static event Action OnPlayerExitBattleZone;
    
    private List<BattleEnemy> _activeEnemies = new List<BattleEnemy>();
    public IReadOnlyList<BattleEnemy> ActiveEnemies => _activeEnemies;
    
    
    private Transform _player;
    private BattleSpawner _battleSpawner;
    private SphereCollider _collider;

    private void OnValidate()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = _useTrigger;
        _collider.radius = _zoneSize;
        _collider.center = _zoneCenter;
    }

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = _useTrigger;
        
        _battleSpawner = GetComponentInChildren<BattleSpawner>();
    }

    private void OnEnable()
    {
        _battleSpawner.OnEnemySpawned += RegisterEnemy;
    }

    private void OnDisable()
    {
        _battleSpawner.OnEnemySpawned -= RegisterEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            _player = player.transform;
            _player.GetComponent<PlayerBattle>().SetBattlerPlatform(this);
            
            OnPlayerEnterBattleZone?.Invoke();
            _battleSpawner.SpawnEnemies(_player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            OnPlayerExitBattleZone?.Invoke();
        }
    }
    
    private void RegisterEnemy(BattleEnemy enemy)
    {
        if (!_activeEnemies.Contains(enemy))
            _activeEnemies.Add(enemy);

        enemy.OnEnemyDied += () => _activeEnemies.Remove(enemy);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawSphere(_zoneCenter, _zoneSize);
    }
}
