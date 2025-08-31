using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _lifeTime = 5f;

    private Vector3 _direction;
    private CustomPool<Projectile> _pool;
    private float _spawnTime;

    public void SetPool(CustomPool<Projectile> pool)
    {
        _pool = pool;
    }

    public void Shoot(Vector3 direction)
    {
        _direction = direction.normalized;
        _spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - _spawnTime > _lifeTime)
        {
            ReturnToPool();
            return;
        }

        transform.position += _direction * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _pool.Release(this);
    }
}
