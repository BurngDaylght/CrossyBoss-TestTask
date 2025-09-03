using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PlayerRoadMovement : MonoBehaviour, IRoadMovable
{
    [Header("Movement Settings")]
    [SerializeField] private float _stepDistance = 1f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _xLimit = 10f;
    [SerializeField] private LayerMask _obstacleLayers = ~0;

    public event Action OnPlayerHitEnemy;
    public event Action OnMoveComplete;

    private Vector3 _targetPosition;
    private bool _isMoving = false;
    [SerializeField] private bool _controlEnabled = false;
    public bool IsMoving => _isMoving;

    private bool _disableAfterMove = false;
    private bool _isCrashing = false;

    private PlayerAnimation _playerAnimation;
    private LevelLogic _levelLogic;
    private BattlePlatform _battlePlatform;
    private Chest _chest;

    [Inject]
    private void Construct(LevelLogic levelLogic, PlayerAnimation playerAnimation, BattlePlatform battlePlatform, Chest chest)
    {
        _levelLogic = levelLogic;
        _playerAnimation = playerAnimation;
        _battlePlatform = battlePlatform;
        _chest = chest;
    }

    private void OnEnable()
    {
        _battlePlatform.OnPlayerEnterBattleZone += DisableControl;
        _levelLogic.OnLevelStart += EnableControl;
        _levelLogic.OnLevelComplete += DisableControl;
        _chest.OnChestInteracted += DisableControl;
    }

    private void OnDisable()
    {
        _battlePlatform.OnPlayerEnterBattleZone -= DisableControl;
        _levelLogic.OnLevelStart -= EnableControl;
        _levelLogic.OnLevelComplete -= DisableControl;
        _chest.OnChestInteracted -= DisableControl;
    }

    private void Update()
    {
        if (_isMoving && _controlEnabled)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                transform.position = _targetPosition;
                _isMoving = false;

                if (_disableAfterMove)
                {
                    _controlEnabled = false;
                    _disableAfterMove = false;
                }

                OnMoveComplete?.Invoke();
            }
        }
    }

    public void MoveStraight() => TryMove(Vector3.forward);
    public void MoveLeft() => TryMove(Vector3.left);
    public void MoveRight() => TryMove(Vector3.right);
    public void MoveBack() => TryMove(Vector3.back);

    private void TryMove(Vector3 direction)
    {
        if (!_controlEnabled || _isMoving || _isCrashing) return;

        float halfHeight = 0.5f;
        float halfWidth = 0.5f;
        Vector3 halfExtents = new Vector3(halfWidth, halfHeight, halfWidth);
        Vector3 casterCenter = transform.position + Vector3.up * halfHeight;

        Vector3 targetPos = transform.position + direction * _stepDistance;
        if (Mathf.Abs(targetPos.x) > _xLimit) return;

        if (Physics.BoxCast(casterCenter, halfExtents, direction, out RaycastHit hitInfo,
                Quaternion.identity, _stepDistance, _obstacleLayers, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider != null && hitInfo.collider.gameObject != gameObject)
            {
                if (hitInfo.collider.TryGetComponent<RoadEnemy>(out RoadEnemy enemy))
                    OnCrashIntoEnemy(enemy, false);

                return;
            }
        }

        _targetPosition = targetPos;
        _isMoving = true;
        _playerAnimation?.PlayRotateAnimation(direction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<RoadEnemy>(out _))
            OnCrashIntoEnemy(null, true);
    }

    private void OnCrashIntoEnemy(RoadEnemy enemy = null, bool isEnemyTouchFirst = false)
    {
        OnPlayerHitEnemy?.Invoke();

        if (_isCrashing) return;
        _isCrashing = true;

        _controlEnabled = false;
        _isMoving = false;
        _targetPosition = transform.position;

        Collider collider = GetComponent<Collider>();
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        if (collider != null) collider.enabled = false;
        if (rigidbody != null) rigidbody.isKinematic = true;

        Vector3? crashTarget = null;
        if (!isEnemyTouchFirst && enemy != null)
        {
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider != null)
                crashTarget = enemyCollider.ClosestPoint(transform.position);
        }

        _playerAnimation.PlayDeathAnimation(crashTarget, () =>
        {
            if (collider != null) collider.enabled = true;
            if (rigidbody != null) rigidbody.isKinematic = false;
            _isCrashing = false;
        });
    }

    private void DisableControl()
    {
        if (_isMoving)
        {
            _disableAfterMove = true;
            return;
        }

        _controlEnabled = false;
    }

    private void EnableControl()
    {
        _levelLogic.OnLevelStart -= EnableControl;

        _controlEnabled = true;
    }
}
