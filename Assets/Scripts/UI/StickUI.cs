using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class StickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform _stickHandle;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Vector2 _startPos;
    private bool _isPressed = false;
    private bool _enabled = true;

    public bool IsPressed => _isPressed;

    private LevelLogic _levelLogic;
    
    [Inject]
    private void Construct(LevelLogic levelLogic)
    {
        _levelLogic = levelLogic;
    }

    private void OnEnable()
    {
        _levelLogic.OnLevelComplete += DisableStick;
    }

    private void OnDisable()
    {
        _levelLogic.OnLevelComplete -= DisableStick;
    }

    private void Awake()
    {
        _startPos = _stickHandle.anchoredPosition;

        HideImmediate();
    }
    
    public void HideImmediate()
    {
        _canvasGroup.alpha = 0f;

        _stickHandle.anchoredPosition = _startPos;
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

        _stickHandle.anchoredPosition = _startPos;
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;

        if (!_enabled)
        {
            _isPressed = false;
            ShowStick(false);

            _stickHandle.anchoredPosition = _startPos;
        }
    }
    
    private void DisableStick()
    {
        SetEnabled(false);
    }
        
    private void EnableStick()
    {
        SetEnabled(false);
    }

    public void ShowStick(bool show)
    {
        _canvasGroup.alpha = show ? 1f : 0f;
    }
}
