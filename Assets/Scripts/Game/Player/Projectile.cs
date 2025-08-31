using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _lifeTime = 5f;

    private Transform _target;
    private CustomPool<Projectile> _pool;
    private float _spawnTime;

    public void SetPool(CustomPool<Projectile> pool)
    {
        _pool = pool;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _spawnTime = Time.time;
    }

    private void Update()
    {
        if (_target == null || Time.time - _spawnTime > _lifeTime)
        {
            ReturnToPool();
            return;
        }

        Vector3 direction = (_target.position - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target == null) return;

        if (other.transform == _target)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
            }

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        _pool.Release(this);
    }
}
