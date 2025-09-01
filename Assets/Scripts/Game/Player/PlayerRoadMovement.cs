using System;
using UnityEngine;
using Zenject;

public class PlayerRoadMovement : MonoBehaviour, IRoadMovable
{
    [SerializeField] private float _stepDistance = 1f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _xLimit = 10f;
    [SerializeField] private LayerMask _obstacleLayers = ~0;
    
    public event Action OnPlayerHitEnemy;

    private Vector3 _targetPosition;
    
    private bool _isMoving = false;
    [SerializeField] private bool _controlEnabled = false;
    
    private PlayerAnimation _playerAnimation;
    private LevelLogic _levelLogic;
    private BattlePlatform _battlePlatform;
    
    [Inject]
    private void Construct(LevelLogic levelLogic, PlayerAnimation playerAnimation, BattlePlatform battlePlatform)
    {
        _levelLogic = levelLogic;
        _playerAnimation = playerAnimation;
        _battlePlatform = battlePlatform;
    }
    
    private void OnEnable()
    {
        _battlePlatform.OnPlayerEnterBattleZone += DisableControl;
        _battlePlatform.OnPlayerExitBattleZone += EnableControl;

        _levelLogic.OnLevelStart += EnableControl;
        _levelLogic.OnLevelComplete += DisableControl;
    }

    private void OnDisable()
    {
        _battlePlatform.OnPlayerEnterBattleZone -= DisableControl;
        _battlePlatform.OnPlayerExitBattleZone -= EnableControl;
        
        _levelLogic.OnLevelStart -= EnableControl;
        _levelLogic.OnLevelComplete -= DisableControl;
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
        if (!_controlEnabled || _isMoving) return;

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
    
    private void DisableControl()
    {
        _controlEnabled = false;
    }

    private void EnableControl()
    {
        _controlEnabled = true;
    }
}
