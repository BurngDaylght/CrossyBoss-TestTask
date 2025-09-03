using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _shrinkDuration = 0.3f;

    private Vector3 _startScale;
    private Vector3 _direction;
    private CustomPool<Projectile> _pool;
    private float _spawnTime;
    private bool _isReturning = false;

    private void OnEnable()
    {
        _startScale = transform.localScale;
    }

    public void SetPool(CustomPool<Projectile> pool)
    {
        _pool = pool;
    }

    public void Shoot(Vector3 direction)
    {
        _direction = direction.normalized;
        _spawnTime = Time.time;
        _isReturning = false;
        transform.localScale = _startScale;
    }

    private void Update()
    {
        if (_isReturning) return;

        if (Time.time - _spawnTime > _lifeTime)
        {
            StartShrinkAndReturn();
            return;
        }

        transform.position += _direction * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isReturning) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }

        ReturnToPool();
    }

    private void StartShrinkAndReturn()
    {
        if (_isReturning) return;

        _isReturning = true;
        transform.DOKill();
        transform.DOScale(Vector3.zero, _shrinkDuration).SetEase(Ease.InBack).OnComplete(ReturnToPool);
    }

    private void ReturnToPool()
    {
        _pool.Release(this);
    }
}
