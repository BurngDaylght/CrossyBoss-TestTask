using UnityEngine;
using DG.Tweening;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private float _rotationDuration = 0.2f;

    public void PlayMoveAnimation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.DORotateQuaternion(targetRotation, _rotationDuration);
    }
    
    private void OnDestroy()
    {
        transform.DOKill();
    }
}
