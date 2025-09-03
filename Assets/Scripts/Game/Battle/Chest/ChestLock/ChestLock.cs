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
    [SerializeField] private Vector3 _punchScale = Vector3.one * 0.25f;
    [SerializeField] private float _punchDuration = 0.5f;
    [SerializeField] private int _punchVibrato = 5;
    [SerializeField] private float _punchElasticity = 0.5f;

    [Header("Wrong Key")]
    [SerializeField] private Color _rejectColor = Color.red;
    [SerializeField] private float _rejectColorDuration = 0.12f;
    [SerializeField] private Vector3 _rejectPunchPosition = new Vector3(10f, 0f, 0f);
    [SerializeField] private float _rejectPunchDuration = 0.35f;
    [SerializeField] private int _rejectPunchVibrato = 10;
    [SerializeField] private float _rejectPunchElasticity = 0.8f;

    private int _currentCount = 0;
    private Sprite[] _acceptableKeySprites;
    private Color _originalColor;

    public event Action OnLockCompleted;

    private void Awake()
    {
        _originalColor = _lockImage.color;
        UpdateCounter();
    }

    public bool CanAcceptKey(Sprite keySprite)
    {
        foreach (Sprite sprite in _acceptableKeySprites)
        {
            if (sprite == keySprite)
            {
                return true;
            }
        }

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
        _counterText.text = $"{_currentCount} / {_requiredKeys}";
    }

    public void SetRandomSprite(Sprite sprite)
    {
        _lockImage.sprite = sprite;
        _currentCount = 0;
        UpdateCounter();
    }

    public void AnimateLock()
    {
        RectTransform lockRect = _lockImage.rectTransform;
        lockRect.DOKill();
        lockRect.DOPunchScale(_punchScale, _punchDuration, _punchVibrato, _punchElasticity);
    }

    public void RejectKeyFeedback()
    {
        _lockImage.DOKill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_lockImage.DOColor(_rejectColor, _rejectColorDuration));
        sequence.Append(_lockImage.DOColor(_originalColor, _rejectColorDuration));

        RectTransform lockRect = _lockImage.rectTransform;
        lockRect.DOKill();
        lockRect.DOPunchPosition(_rejectPunchPosition, _rejectPunchDuration, _rejectPunchVibrato, _rejectPunchElasticity);
    }
}
