using UnityEngine;
using DG.Tweening;
using System;

public class BattleEnemy : EnemyBase, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _attackCooldown = 1f;

    [Header("Attack animation")]
    [SerializeField] private float _attackAnimScale = 1.3f;
    [SerializeField] private float _attackAnimDuration = 0.2f;

    [Header("Death animation")]
    [SerializeField] private float _deathDuration = 0.6f;
    [SerializeField] private float _deathRise = 0.5f;
    [SerializeField] private float _deathRotateDegrees = 90f;

    [Header("Spawn animation")]
    [SerializeField] private bool _playSpawnAnimation = true;
    [SerializeField] private float _spawnDuration = 0.5f;
    [SerializeField] private float _spawnRise = 0.3f;
    [SerializeField] private float _spawnFromScale = 0.1f;

    private HealthUI _healthUI;
    private float _currentHealth;
    private Transform _player;
    private CustomPool<BattleEnemy> _pool;
    private float _lastAttackTime = 0f;
    private Rigidbody _rb;
    private Collider[] _colliders;

    private Vector3 _initialScale;
    private Quaternion _initialRotation;

    private bool _isDying = false;
    private bool _isSpawning = false;

    public event Action OnEnemyDied;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _rb = GetComponent<Rigidbody>();
        _colliders = GetComponentsInChildren<Collider>(true);
        _healthUI = GetComponentInChildren<HealthUI>();

        _initialScale = transform.localScale;
        _initialRotation = transform.rotation;
    }

    protected override void Start()
    {
        base.Start();

        _healthUI.Init(transform, _maxHealth);
    }

    private void OnEnable()
    {
        _currentHealth = _maxHealth;
        _isDying = false;

        transform.rotation = _initialRotation;

        transform.DOKill();

        if (_playSpawnAnimation)
        {
            StartSpawnAnimation();
        }
        else
        {
            FinishSpawnImmediate();
        }
    }

    private void FinishSpawnImmediate()
    {
        foreach (var c in _colliders) c.enabled = true;
        _rb.isKinematic = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.localScale = _initialScale;

        Vector3 pos = transform.position;
        pos.y = 0.5f;
        transform.position = pos;

        _isSpawning = false;
    }

    private void StartSpawnAnimation()
    {
        if (_isDying) return;
        
        _healthUI.Show();

        _isSpawning = true;

        foreach (var c in _colliders) c.enabled = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        Vector3 targetPos = transform.position;
        targetPos.y = 0.5f;

        Vector3 startPos = targetPos - new Vector3(0f, _spawnRise, 0f);
        transform.position = startPos;

        transform.localScale = _initialScale * _spawnFromScale;

        transform.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(targetPos.y, _spawnDuration).SetEase(Ease.OutBack));
        seq.Join(transform.DOScale(_initialScale, _spawnDuration).SetEase(Ease.OutBack));
        seq.OnComplete(() =>
        {
            foreach (var c in _colliders) c.enabled = true;
            _rb.isKinematic = false;
            _isSpawning = false;
        });
        seq.Play();
    }

    public void SetPlayerTarget(Transform player)
    {
        _player = player;
    }

    private void FixedUpdate()
    {
        if (_player == null || _isDying || _isSpawning) return;

        Vector3 raw = _player.position - transform.position;
        raw.y = 0f;
        Vector3 dirToPlayer = raw.normalized;

        Vector3 a = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 b = new Vector3(_player.position.x, 0f, _player.position.z);
        float distance = Vector3.Distance(a, b);

        if (distance <= _attackRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleToPlayer <= 5f)
            {
                TryAttackPlayer();
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            return;
        }

        Vector3 newPos = _rb.position + dirToPlayer * _movementSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(newPos);

        if (dirToPlayer.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirToPlayer, Vector3.up);
            Quaternion slerp = Quaternion.Slerp(_rb.rotation, targetRot, 10f * Time.fixedDeltaTime);
            _rb.MoveRotation(slerp);
        }
    }

    private void TryAttackPlayer()
    {
        if (Time.time - _lastAttackTime < _attackCooldown) return;

        AttackPlayer();
        _lastAttackTime = Time.time;
    }

    private void AttackPlayer()
    {
        if (_player == null || _isDying || _isSpawning) return;

        PlayAttackAnimation();

        IDamageable damageable = _player.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }
    }

    private void PlayAttackAnimation()
    {
        transform.DOKill();
        Vector3 originalScale = transform.localScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originalScale * _attackAnimScale, _attackAnimDuration).SetEase(Ease.OutBack))
           .Append(transform.DOScale(originalScale, _attackAnimDuration).SetEase(Ease.InBack));
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
        if (_isDying || _isSpawning) return;

        _currentHealth -= damage;
        _healthUI.UpdateHealth(_currentHealth);

        if (_currentHealth <= 0f)
        {
            _healthUI.Hide();
            Die();
        }
    }

    private void Die()
    {
        if (_isDying) return;
        _isDying = true;

        OnEnemyDied?.Invoke();

        PlayDeathAnimation();
    }

    private void PlayDeathAnimation()
    {
        transform.DOKill();

        foreach (var c in _colliders) c.enabled = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveY(transform.position.y + _deathRise, _deathDuration * 0.5f).SetEase(Ease.OutCubic));
        seq.Join(transform.DORotate(new Vector3(0f, _deathRotateDegrees, 0f), _deathDuration, RotateMode.LocalAxisAdd));
        seq.Join(transform.DOScale(Vector3.zero, _deathDuration).SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            _rb.isKinematic = false;
            ReturnToPool();
        });

        seq.Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
