using System;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerBattle : MonoBehaviour, IBattleMovable
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;
    [SerializeField] private float _confinementPadding = 0.05f;

    private bool _inBattle = false;
    public bool InBattle => _inBattle;

    private bool _controlEnabled = false;
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;

    public event Action OnStartMoving;
    public event Action OnStopMoving;

    private BattlePlatform _currentPlatform;
    private HealthUI _healthUI;
    private PlayerAnimation _playerAnimation;
    private BattlePlatform _battlePlatform;
    private LevelLogic _levelLogic;
    private Chest _chest;
    private PlayerRoadMovement _playerRoadMovement;

    [Inject]
    private void Construct(
        PlayerAnimation playerAnimation,
        LevelLogic levelLogic,
        BattlePlatform battlePlatform,
        Chest chest,
        PlayerRoadMovement playerRoadMovement)
    {
        _playerAnimation = playerAnimation;
        _levelLogic = levelLogic;
        _battlePlatform = battlePlatform;
        _chest = chest;
        _playerRoadMovement = playerRoadMovement;
    }

    private void OnEnable()
    {
        _battlePlatform.OnPlayerEnterBattleZone += HandleEnterBattleZone;
        _battlePlatform.OnPlayerExitBattleZone += HandleExitBattleZone;

        _levelLogic.OnLevelComplete += DisableControl;
        _levelLogic.OnLevelLosing += DisableControl;

        _chest.OnChestInteracted += DisableControl;
    }

    private void OnDisable()
    {
        _battlePlatform.OnPlayerEnterBattleZone -= HandleEnterBattleZone;
        _battlePlatform.OnPlayerExitBattleZone -= HandleExitBattleZone;

        _levelLogic.OnLevelComplete -= DisableControl;
        _levelLogic.OnLevelLosing -= DisableControl;

        _chest.OnChestInteracted -= DisableControl;

        _playerRoadMovement.OnMoveComplete -= OnRoadMoveComplete_EnableControl;
    }
    
    private void Start()
    {
        _healthUI = GetComponentInChildren<HealthUI>();
    }

    private void Update()
    {
        if (_inBattle)
            LookAtNearestEnemy();
    }

    public void EnterBattle()
    {
        _inBattle = true;
    }

    public void Move(Vector2 direction)
    {
        if (!_inBattle || !_controlEnabled) return;

        bool wasMoving = _isMoving;
        _isMoving = direction != Vector2.zero;

        if (_isMoving && !wasMoving) OnStartMoving?.Invoke();
        else if (!_isMoving && wasMoving) OnStopMoving?.Invoke();

        if (!_isMoving) return;

        Vector3 moveDir = new Vector3(direction.x, 0f, direction.y).normalized;
        Vector3 desiredPos = transform.position + moveDir * _moveSpeed * Time.deltaTime;

        desiredPos = ClampPositionToPlatform(desiredPos);

        transform.position = desiredPos;
        _playerAnimation?.PlayRotateAnimation(moveDir);
    }

    private Vector3 ClampPositionToPlatform(Vector3 desiredPos)
    {
        Vector3 center = _currentPlatform.GetWorldCenter();
        float radius = _currentPlatform.GetWorldRadius() - _confinementPadding;

        Vector3 offset = new Vector3(desiredPos.x, 0f, desiredPos.z) - new Vector3(center.x, 0f, center.z);
        float distance = offset.magnitude;

        if (distance > radius)
        {
            Vector3 clampedOffset = offset.normalized * radius;
            return new Vector3(center.x + clampedOffset.x, transform.position.y, center.z + clampedOffset.z);
        }

        return desiredPos;
    }

    private void HandleEnterBattleZone()
    {
        EnterBattle();
        _healthUI.Show(1f);

        if (_playerRoadMovement.IsMoving)
            _playerRoadMovement.OnMoveComplete += OnRoadMoveComplete_EnableControl;
        else
            EnableControl();
    }

    private void OnRoadMoveComplete_EnableControl()
    {
        _playerRoadMovement.OnMoveComplete -= OnRoadMoveComplete_EnableControl;

        EnableControl();
    }

    private void HandleExitBattleZone()
    {
        _currentPlatform = null;
        DisableControl();
    }

    private void LookAtNearestEnemy()
    {
        if (_currentPlatform.ActiveEnemies.Count == 0) return;

        BattleEnemy nearest = _currentPlatform.ActiveEnemies
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();

        if (nearest == null) return;

        Vector3 lookDir = nearest.transform.position - transform.position;
        lookDir.y = 0;

        if (lookDir.sqrMagnitude > 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
        }
    }

    public void SetBattlerPlatform(BattlePlatform battlePlatform)
    {
        _currentPlatform = battlePlatform;
    }

    public BattleEnemy GetNearestEnemy()
    {
        if (_currentPlatform == null || _currentPlatform.ActiveEnemies.Count == 0) return null;

        return _currentPlatform.ActiveEnemies
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();
    }

    private void DisableControl() => _controlEnabled = false;
    private void EnableControl() => _controlEnabled = true;
}
