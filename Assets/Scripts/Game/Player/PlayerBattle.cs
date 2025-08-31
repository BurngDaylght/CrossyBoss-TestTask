using UnityEngine;
using Zenject;
using System.Linq;
using System;

public class PlayerBattle : MonoBehaviour, IBattleMovable
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;

    private bool _inBattle = false;
    public bool InBattle => _inBattle;
    
    private bool _controlEnabled = false;
    
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;
    
    public event Action OnStartMoving;
    public event Action OnStopMoving;

    private PlayerAnimation _playerAnimation;
    private BattlePlatform _currentPlatform;
    
    private void OnEnable()
    {
        BattlePlatform.OnPlayerEnterBattleZone += HandleEnterBattleZone;
        BattlePlatform.OnPlayerExitBattleZone += HandleExitBattleZone;
    }

    private void OnDisable()
    {
        BattlePlatform.OnPlayerEnterBattleZone -= HandleEnterBattleZone;
        BattlePlatform.OnPlayerExitBattleZone -= HandleExitBattleZone;
    }

    private void Awake()
    {
        _playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (_inBattle)
        {
            LookAtNearestEnemy();
        }
    }

    public void EnterBattle()
    {
        _inBattle = true;
        Debug.Log("Игрок вошёл в боевой режим!");
    }

    public void Move(Vector2 direction)
    {
        if (!_inBattle || !_controlEnabled) return;

        bool wasMoving = _isMoving;
        _isMoving = direction != Vector2.zero;

        if (_isMoving && !wasMoving)
            OnStartMoving?.Invoke();
        else if (!_isMoving && wasMoving)
            OnStopMoving?.Invoke();

        if (_isMoving)
        {
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y).normalized;
            transform.position += moveDirection * _moveSpeed * Time.deltaTime;

            _playerAnimation?.PlayMoveAnimation(moveDirection);
        }
    }

    private void HandleEnterBattleZone()
    {
        EnterBattle();
        EnableControl(true);
    }

    private void HandleExitBattleZone()
    {
        _currentPlatform = null;
        EnableControl(false);
    }

    private void LookAtNearestEnemy()
    {
        if (_currentPlatform == null || _currentPlatform.ActiveEnemies.Count == 0)
            return;

        BattleEnemy nearest = _currentPlatform.ActiveEnemies
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();

        if (nearest == null) return;

        Vector3 lookDir = (nearest.transform.position - transform.position).normalized;
        lookDir.y = 0;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
        }
    }
    
    public void SetBattlerPlatform(BattlePlatform battlePlatform)
    {
        _currentPlatform = battlePlatform;
    }
    
    public BattleEnemy GetNearestEnemy()
    {
        if (_currentPlatform == null || _currentPlatform.ActiveEnemies.Count == 0)
            return null;

        return _currentPlatform.ActiveEnemies
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();
    }

    public void EnableControl(bool enabled)
    {
        _controlEnabled = enabled;
    }
}
