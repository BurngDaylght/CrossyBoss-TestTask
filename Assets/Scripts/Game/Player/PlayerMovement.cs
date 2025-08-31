using System;
using UnityEngine;
using Zenject;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] private float _stepDistance = 1f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _xLimit = 10f;
    [SerializeField] private LayerMask _obstacleLayers = ~0;
    
    public event Action OnPlayerHitEnemy;

    private Vector3 _targetPosition;
    
    private bool _isMoving = false;
    private bool _controlEnabled = true;
    
    private PlayerAnimation _playerAnimation;
    
    private void Awake()
    {
        _playerAnimation = GetComponent<PlayerAnimation>();
    }
    
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

    private void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                transform.position = _targetPosition;
                _isMoving = false;
            }
        }
    }

    public void MoveStraight() => TryMove(Vector3.forward);
    public void MoveLeft() => TryMove(Vector3.left);
    public void MoveRight() => TryMove(Vector3.right);
    public void MoveBack() => TryMove(Vector3.back);

    private void TryMove(Vector3 direction)
    {
        if (_isMoving) return;

        float halfHeight = 0.5f;
        float halfWidth = 0.5f;
        Vector3 halfExtents = new Vector3(halfWidth, halfHeight, halfWidth);
        Vector3 casterCenter = transform.position + Vector3.up * halfHeight;

        Vector3 targetPos = transform.position + direction * _stepDistance;
        if (Mathf.Abs(targetPos.x) > _xLimit) return;

        if (Physics.BoxCast(casterCenter, halfExtents, direction, out RaycastHit hitInfo,
                            Quaternion.identity, _stepDistance, _obstacleLayers.value, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider != null && hitInfo.collider.gameObject != gameObject)
            {
                if (hitInfo.collider.TryGetComponent<RoadEnemy>(out RoadEnemy enemy))
                {
                    OnPlayerHitEnemy?.Invoke();
                    return;
                }

                Debug.Log("Путь заблокирован препятствием: " + hitInfo.collider.name);
                return;
            }
        }

        _targetPosition = targetPos;
        _isMoving = true;
        _playerAnimation?.PlayMoveAnimation(direction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<RoadEnemy>(out RoadEnemy enemy))
        {
            OnPlayerHitEnemy?.Invoke();
        }
    }
    
    private void HandleEnterBattleZone()
    {
        EnableControl(false);
    }

    private void HandleExitBattleZone()
    {
        EnableControl(true);
    }

    public void EnableControl(bool enabled)
    {
        _controlEnabled = enabled;
    }
}
