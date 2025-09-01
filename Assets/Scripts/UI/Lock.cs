using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChestLock : MonoBehaviour
{
    [SerializeField] private Image _lockImage;
    [SerializeField] private TextMeshProUGUI _counterText;
    [SerializeField] private int _requiredKeys = 3;

    private int _currentCount = 0;
    private Color _lockColor;

    public event Action OnLockCompleted;

    private void Awake()
    {
        _lockColor = _lockImage.color;
        UpdateCounter();
    }

    public bool CanAcceptKey(Color keyColor)
    {
        return keyColor == _lockColor;
    }

    public void AddKey(Color keyColor)
    {
        if (!CanAcceptKey(keyColor)) return;

        _currentCount++;
        UpdateCounter();

        if (_currentCount >= _requiredKeys)
        {
            OnLockCompleted?.Invoke();
        }
    }

    private void UpdateCounter()
    {
        _counterText.text = $"{_currentCount} / {_requiredKeys}";
    }

    public void SetRandomColor(Color color)
    {
        _lockColor = color;
        _lockImage.color = new Color(color.r, color.g, color.b, 1f);
        _currentCount = 0;
        UpdateCounter();
    }   
}
