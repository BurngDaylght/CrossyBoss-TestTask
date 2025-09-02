using DG.Tweening;
using UnityEngine;

public class RoadEnemy : EnemyBase
{
    private CustomPool<RoadEnemy> _pool;

    public void SetMoveDirection(Vector3 direction)
    {
        _moveDirection = direction.normalized;
    }

    public void SetSpeed(float speed)
    {
        _movementSpeed = speed;
    }

    protected override void Update()
    {
        base.Update();

        Move();
        CheckBounds();
    }
    
    
    private void CheckBounds()
    {
        if (Mathf.Abs(transform.position.x) > _xLimit)
        {
            transform.DOKill();
            ReturnToPool();
        }
    }
    
    public void SetPool(CustomPool<RoadEnemy> pool)
    {
        _pool = pool;

        CancelInvoke();
    }

    private void ReturnToPool()
    {
        _pool.Release(this);
    }
}
