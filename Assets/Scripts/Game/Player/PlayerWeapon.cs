using UnityEngine;
using Zenject;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _shotPoint;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private int _prewarmCount = 20;
    [SerializeField] private Transform _poolParent;

    private float _nextFireTime;
    private bool _canShoot = true;

    private PlayerBattle _playerBattle;
    private LevelLogic _levelLogic;
    private CustomPool<Projectile> _projectilePool;

    [Inject]
    private void Construct(PlayerBattle playerBattle, LevelLogic levelLogic)
    {
        _playerBattle = playerBattle;
        _levelLogic = levelLogic;
    }

    private void Awake()
    {
        _projectilePool = new CustomPool<Projectile>(_projectilePrefab, _prewarmCount, _poolParent);
    }

    private void OnEnable()
    {
        _playerBattle.OnStartMoving += HandleStartShoot;
        _playerBattle.OnStopMoving += HandleStopShoot;
        _levelLogic.OnLevelLosing += HandleStopShoot;
    }

    private void OnDisable()
    {
        _playerBattle.OnStartMoving -= HandleStartShoot;
        _playerBattle.OnStopMoving -= HandleStopShoot;
        _levelLogic.OnLevelLosing -= HandleStopShoot;
    }

    private void Update()
    {
        if (!_canShoot) return;

        BattleEnemy target = _playerBattle.GetNearestEnemy();
        if (target == null) return;

        if (Time.time >= _nextFireTime && IsFacingTarget(target))
        {
            Shoot(target);
            _nextFireTime = Time.time + _fireRate;
        }
    }

    private bool IsFacingTarget(BattleEnemy target)
    {
        Vector3 toTarget = target.transform.position - _shotPoint.position;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        if (toTargetXZ.sqrMagnitude < 0.0001f) return true;

        Vector3 forwardXZ = new Vector3(_playerBattle.transform.forward.x, 0f, _playerBattle.transform.forward.z).normalized;
        float angle = Vector3.Angle(forwardXZ, toTargetXZ.normalized);

        return angle <= 5f;
    }

    private void Shoot(BattleEnemy target)
    {
        Projectile projectile = _projectilePool.Get();
        Vector3 dir = (target.transform.position - _shotPoint.position).normalized;

        projectile.transform.position = _shotPoint.position + dir * 0.1f;
        projectile.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        projectile.SetPool(_projectilePool);
        projectile.Shoot(dir);
    }

    private void HandleStartShoot() => _canShoot = false;
    private void HandleStopShoot() => _canShoot = true;
}
