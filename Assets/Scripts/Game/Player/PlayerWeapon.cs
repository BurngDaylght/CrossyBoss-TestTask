using UnityEngine;
using Zenject;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _shotPoint;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private int _prewarmCount = 20;
    [SerializeField] private Transform _poolParent;

    private float _nextFireTime;
    private PlayerBattle _playerBattle;
    private CustomPool<Projectile> _projectilePool;

    private bool _canShoot = true;

    [Inject]
    private void Construct(PlayerBattle playerBattle)
    {
        _playerBattle = playerBattle;
    }

    private void Awake()
    {
        _projectilePool = new CustomPool<Projectile>(_projectilePrefab, _prewarmCount, _poolParent);
    }

    private void OnEnable()
    {
        _playerBattle.OnStartMoving += HandleStartMoving;
        _playerBattle.OnStopMoving += HandleStopMoving;
    }
    
    private void OnDisable()
    {
        _playerBattle.OnStartMoving -= HandleStartMoving;
        _playerBattle.OnStopMoving -= HandleStopMoving;
    }

    private void Update()
    {
        if (!_canShoot) return;

        BattleEnemy target = _playerBattle.GetNearestEnemy();
        if (target == null) return;

        if (Time.time >= _nextFireTime)
        {
            Shoot(target);
            _nextFireTime = Time.time + _fireRate;
        }
    }

    private void Shoot(BattleEnemy target)
    {
        Projectile projectile = _projectilePool.Get();

        projectile.transform.position = _shotPoint.position;
        projectile.transform.rotation = Quaternion.identity;

        projectile.SetPool(_projectilePool);
        projectile.SetTarget(target.transform);
    }

    private void HandleStartMoving()
    {
        _canShoot = false;
    }

    private void HandleStopMoving()
    {
        _canShoot = true;
    }
}
