using UnityEngine;
using UnityEngine.EventSystems;

public class StickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform stickHandle;
    [SerializeField] private CanvasGroup canvasGroup;

    private Vector2 _startPos;
    private bool _isPressed = false;
    private bool _enabled = true;

    public bool IsPressed => _isPressed;

    private void Awake()
    {
        if (stickHandle != null)
            _startPos = stickHandle.anchoredPosition;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_enabled) return;

        _isPressed = true;
        ShowStick(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_enabled) return;

        _isPressed = false;
        ShowStick(false);

        if (stickHandle != null)
            stickHandle.anchoredPosition = _startPos;
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;

        if (!_enabled)
        {
            _isPressed = false;
            ShowStick(false);

            if (stickHandle != null)
                stickHandle.anchoredPosition = _startPos;
        }
    }

    private void ShowStick(bool show)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = show ? 1f : 0f;
    }
}
