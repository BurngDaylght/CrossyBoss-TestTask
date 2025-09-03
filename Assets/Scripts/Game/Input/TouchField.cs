using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TouchField : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Tap & Swipe Settings")]
    [SerializeField] private float _tapMoveThreshold = 15f;
    [SerializeField] private float _tapTimeThreshold = 0.25f;

    public Vector2 SwipeDelta { get; private set; }
    public bool WasTap { get; private set; }
    public bool WasSwipe { get; private set; }

    private Vector2 _startTouch;
    private Vector2 _currentDragDelta;
    private float _touchStartTime;

    private bool _isSwiping;
    private bool _isEnabled = true;

    private LevelLogic _levelLogic;

    [Inject]
    private void Construct(LevelLogic levelLogic)
    {
        _levelLogic = levelLogic;
    }

    private void OnEnable()
    {
        _levelLogic.OnLevelStart += EnableInput;
        _levelLogic.OnLevelComplete += DisableInput;
    }

    private void OnDisable()
    {
        _levelLogic.OnLevelStart -= EnableInput;
        _levelLogic.OnLevelComplete -= DisableInput;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isEnabled) return;

        _isSwiping = true;
        _startTouch = eventData.position;
        _touchStartTime = Time.time;
        _currentDragDelta = Vector2.zero;

        WasTap = false;
        WasSwipe = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isEnabled || !_isSwiping) return;

        _currentDragDelta = eventData.position - _startTouch;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isEnabled || !_isSwiping) return;

        Vector2 endTouch = eventData.position;
        Vector2 delta = endTouch - _startTouch;
        float duration = Time.time - _touchStartTime;

        if (delta.magnitude <= _tapMoveThreshold && duration <= _tapTimeThreshold)
        {
            WasTap = true;
            WasSwipe = false;
            SwipeDelta = Vector2.zero;
        }
        else
        {
            WasTap = false;
            WasSwipe = true;
            SwipeDelta = delta;
        }

        _isSwiping = false;
        _currentDragDelta = Vector2.zero;
    }

    public void ConsumeTap()
    {
        WasTap = false;
    }

    public void ConsumeSwipe()
    {
        WasSwipe = false;
        SwipeDelta = Vector2.zero;
    }

    public Vector2 GetCurrentDragDelta()
    {
        return _currentDragDelta;
    }

    public void EnableInput()
    {
        _isEnabled = true;
    }

    public void DisableInput()
    {
        _isEnabled = false;
        ResetInputState();
    }

    private void ResetInputState()
    {
        WasTap = false;
        WasSwipe = false;
        SwipeDelta = Vector2.zero;
        _isSwiping = false;
        _currentDragDelta = Vector2.zero;
    }
}
