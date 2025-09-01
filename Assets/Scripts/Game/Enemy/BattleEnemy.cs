using UnityEngine;
using DG.Tweening;
using System;

public class BattleEnemy : EnemyBase, IDamageable
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _attackCooldown = 1f;
    
    private float _currentHealth;

    private Transform _player;
    private CustomPool<BattleEnemy> _pool;
    private float _lastAttackTime = 0f;

    public event Action OnEnemyDied;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void SetPlayerTarget(Transform player)
    {
        _player = player;
    }

    protected override void Move()
    {
        if (_player == null) return;

        _moveDirection = (_player.position - transform.position).normalized;
        transform.position += _moveDirection * _movementSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance <= _attackRange)
            TryAttackPlayer();
    }

    private void TryAttackPlayer()
    {
        if (Time.time - _lastAttackTime < _attackCooldown) return;
        AttackPlayer();
        _lastAttackTime = Time.time;
    }

    private void AttackPlayer()
    {
        if (_player == null) return;

        IDamageable damageable = _player.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }
    }

    public void SetPool(CustomPool<BattleEnemy> pool)
    {
        _pool = pool;
    }

    private void ReturnToPool()
    {
        transform.DOKill();
        _pool.Release(this);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        
        if (_currentHealth <= 0)
        {
            OnEnemyDied?.Invoke();
            ReturnToPool();
        }
    }
}
