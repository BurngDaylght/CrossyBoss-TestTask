using UnityEngine;
using DG.Tweening;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float _movementSpeed;
    [SerializeField] protected float _xLimit = 10f;
    [SerializeField] private float _rotationSpeed = 100f;

    protected Vector3 _moveDirection;
    
    protected virtual void Start()
    {
        if (_moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            transform.rotation = targetRotation;
        }
    }
    
    protected virtual void Update()
    {
        Move();
        RotateTowardsMoveDirection();
    }

    protected virtual void Move()
    {
        transform.position += _moveDirection * _movementSpeed * Time.deltaTime;
    }

    private void RotateTowardsMoveDirection()
    {
        if (_moveDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}
