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
    private Rigidbody _rigidbody;
    private Collider[] _colliders;

    private Vector3 _initialScale;
    private Quaternion _initialRotation;

    private bool _isDying = false;
    private bool _isSpawning = false;

    public event Action OnEnemyDied;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _rigidbody = GetComponent<Rigidbody>();
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
            StartSpawnAnimation();
        else
            FinishSpawnImmediate();
    }

    private void FinishSpawnImmediate()
    {
        foreach (var c in _colliders) c.enabled = true;

        _rigidbody.isKinematic = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

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

        _rigidbody.isKinematic = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        Vector3 targetPos = transform.position;
        targetPos.y = 0.5f;
        Vector3 startPos = targetPos - new Vector3(0f, _spawnRise, 0f);
        transform.position = startPos;
        transform.localScale = _initialScale * _spawnFromScale;

        transform.DOKill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(targetPos.y, _spawnDuration).SetEase(Ease.OutBack));
        sequence.Join(transform.DOScale(_initialScale, _spawnDuration).SetEase(Ease.OutBack));
        sequence.OnComplete(() =>
        {
            foreach (var c in _colliders) c.enabled = true;
            _rigidbody.isKinematic = false;
            _isSpawning = false;
        });
        sequence.Play();
    }

    public void SetPlayerTarget(Transform player)
    {
        _player = player;
    }

    protected override void Update()
    {
        base.Update();
    
        if (_isDying || _isSpawning) return;

        Vector3 dir = _player.position - transform.position;
        dir.y = 0f;
        Vector3 dirNormalized = dir.normalized;

        float distance = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(_player.position.x, 0f, _player.position.z)
        );

        if (distance <= _attackRange)
        {
            float angle = Vector3.Angle(transform.forward, dirNormalized);
            if (angle <= 5f)
            {
                TryAttackPlayer();
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            return;
        }

        Vector3 newPos = _rigidbody.position + dirNormalized * _movementSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(newPos);

        if (dirNormalized.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirNormalized, Vector3.up);
            Quaternion slerp = Quaternion.Slerp(_rigidbody.rotation, targetRot, 10f * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(slerp);
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
        if (_isDying || _isSpawning) return;

        PlayAttackAnimation();

        IDamageable damageable = _player.GetComponent<IDamageable>();
        damageable?.TakeDamage(_damage);
    }

    private void PlayAttackAnimation()
    {
        transform.DOKill();
        Vector3 originalScale = transform.localScale;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(originalScale * _attackAnimScale, _attackAnimDuration).SetEase(Ease.OutBack))
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

        _rigidbody.isKinematic = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(transform.position.y + _deathRise, _deathDuration * 0.5f).SetEase(Ease.OutCubic));
        sequence.Join(transform.DORotate(new Vector3(0f, _deathRotateDegrees, 0f), _deathDuration, RotateMode.LocalAxisAdd));
        sequence.Join(transform.DOScale(Vector3.zero, _deathDuration).SetEase(Ease.InBack));

        sequence.OnComplete(() =>
        {
            _rigidbody.isKinematic = false;
            ReturnToPool();
        });

        sequence.Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
