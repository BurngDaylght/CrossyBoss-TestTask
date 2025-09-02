using UnityEngine;
using DG.Tweening;
using System;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private float _rotationDuration = 0.25f;

    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
    }

    public void PlayRotateAnimation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.DORotateQuaternion(targetRotation, _rotationDuration);
    }

    public void PlayDeathAnimation(Vector3? targetPosition, Action onComplete)
    {
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        if (targetPosition.HasValue)
        {
            float approachOffset = 0.3f;

            Vector3 dirToEnemy = (targetPosition.Value - transform.position).normalized;
            Vector3 crashTarget = targetPosition.Value - dirToEnemy * approachOffset;

            float distance = Vector3.Distance(transform.position, crashTarget);
            float speed = 5f;
            float moveDuration = Mathf.Clamp(distance / speed, 0.05f, 0.5f);

            seq.Append(transform.DOMove(crashTarget, moveDuration).SetEase(Ease.OutQuad));
        }

        seq.Append(transform.DOScale(new Vector3(_initialScale.x * 1.4f, _initialScale.y * 1.4f, _initialScale.z * 0.2f), 0.25f).SetEase(Ease.OutBack))
        .AppendInterval(0.15f)
        .Append(transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack))
        .OnComplete(() => onComplete?.Invoke());
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
