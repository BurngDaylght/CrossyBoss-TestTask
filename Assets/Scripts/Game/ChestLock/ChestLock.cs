using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class ChestLock : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _lockImage;
    [SerializeField] private TextMeshProUGUI _counterText;
    [SerializeField] private int _requiredKeys = 3;

    [Header("Animation Settings")]
    [SerializeField] private Vector3 punchScale = Vector3.one * 0.25f;
    [SerializeField] private float punchDuration = 0.25f;
    [SerializeField] private int punchVibrato = 5;
    [SerializeField] private float punchElasticity = 0.5f;

    [Header("Reject (Wrong Key) Feedback")]
    [SerializeField] private Color rejectColor = Color.red;
    [SerializeField] private float rejectColorDuration = 0.12f;
    [SerializeField] private Vector3 rejectPunchPosition = new Vector3(10f, 0f, 0f);
    [SerializeField] private float rejectPunchDuration = 0.35f;
    [SerializeField] private int rejectPunchVibrato = 10;
    [SerializeField] private float rejectPunchElasticity = 0.8f;

    private int _currentCount = 0;
    private Sprite _lockSprite;
    private Sprite[] _acceptableKeySprites;
    private Color _originalColor;

    public event Action OnLockCompleted;

    private void Awake()
    {
        _lockSprite = _lockImage.sprite;
        _originalColor = _lockImage.color;
        UpdateCounter();
    }

    public bool CanAcceptKey(Sprite keySprite)
    {
        if (_acceptableKeySprites == null || _acceptableKeySprites.Length == 0) return false;
        foreach (var s in _acceptableKeySprites)
            if (s == keySprite) return true;
        return false;
    }

    public void SetAcceptableKeys(Sprite[] keys)
    {
        _acceptableKeySprites = keys;
    }

    public void AddKey(Sprite keySprite)
    {
        if (!CanAcceptKey(keySprite)) return;

        _currentCount++;
        UpdateCounter();

        AnimateLock();

        if (_currentCount >= _requiredKeys)
        {
            OnLockCompleted?.Invoke();
        }
    }

    private void UpdateCounter()
    {
        if (_counterText != null)
            _counterText.text = $"{_currentCount} / {_requiredKeys}";
    }

    public void SetRandomSprite(Sprite sprite)
    {
        _lockSprite = sprite;
        _lockImage.sprite = sprite;
        _currentCount = 0;
        UpdateCounter();
    }

    public void AnimateLock()
    {
        RectTransform lockRect = _lockImage.rectTransform;
        lockRect.DOKill();
        lockRect.DOPunchScale(punchScale, punchDuration, punchVibrato, punchElasticity);
    }

    public void RejectKeyFeedback()
    {
        _lockImage.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(_lockImage.DOColor(rejectColor, rejectColorDuration));
        seq.Append(_lockImage.DOColor(_originalColor, rejectColorDuration));

        RectTransform lockRect = _lockImage.rectTransform;
        lockRect.DOKill();
        lockRect.DOPunchPosition(rejectPunchPosition, rejectPunchDuration, rejectPunchVibrato, rejectPunchElasticity);
    }
}
