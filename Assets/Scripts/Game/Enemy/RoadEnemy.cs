using UnityEngine;

public class RoadEnemy : EnemyBase
{
    public void SetMoveDirection(Vector3 direction)
    {
        _moveDirection = direction.normalized;
    }

    public void SetSpeed(float speed)
    {
        _movementSpeed = speed;
    }

    public override void AttackPlayer()
    {
 
    }
}
