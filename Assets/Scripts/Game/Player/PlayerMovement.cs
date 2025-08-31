using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovable
{
    [SerializeField] private float _stepDistance = 1f;
    [SerializeField] private float _moveSpeed = 5f;

    private Vector3 _targetPosition;
    private bool _isMoving = false;

    private void Awake()
    {
        _targetPosition = transform.position;
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

    public void MoveStraight()
    {
        if (!_isMoving)
            StartMove(Vector3.forward);
    }

    public void MoveLeft()
    {
        if (!_isMoving)
            StartMove(Vector3.left);
    }

    public void MoveRight()
    {
        if (!_isMoving)
            StartMove(Vector3.right);
    }

    public void MoveBack()
    {
        if (!_isMoving)
            StartMove(Vector3.back);
    }

    private void StartMove(Vector3 direction)
    {
        _targetPosition = transform.position + direction * _stepDistance;
        _isMoving = true;
    }
}
