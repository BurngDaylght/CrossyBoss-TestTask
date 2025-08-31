using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] private float _stepDistance = 1f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _collisionCheckDistance = 0.9f;

    private Vector3 _targetPosition;
    private bool _isMoving = false;
    
    private PlayerAnimation _playerAnimation;
    
    private void Awake()
    {
        _playerAnimation = GetComponent<PlayerAnimation>();
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
        float halfHeight = 0.5f;
        float halfWidth = 0.5f;
        Vector3 center = transform.position + Vector3.up * halfHeight;

        if (!_isMoving && !Physics.BoxCast(center, new Vector3(halfWidth, halfHeight, halfWidth), direction, out _, Quaternion.identity, _stepDistance))
        {
            _targetPosition = transform.position + direction * _stepDistance;
            _isMoving = true;
            _playerAnimation?.PlayMoveAnimation(direction);
        }
    }
}