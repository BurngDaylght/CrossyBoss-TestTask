using UnityEngine;
using UnityEngine.EventSystems;

public class TouchField : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Vector2 SwipeDelta { get; private set; } = Vector2.zero;
    public bool WasTap { get; private set; } = false;
    public bool WasSwipe { get; private set; } = false;

    [SerializeField] private float _tapMoveThreshold = 15f;
    [SerializeField] private float _tapTimeThreshold = 0.25f;

    private Vector2 _startTouch;
    private float _touchStartTime;
    private Vector2 _currentDragDelta;
    private bool _isSwiping = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isSwiping = true;
        _startTouch = eventData.position;
        _touchStartTime = Time.time;
        _currentDragDelta = Vector2.zero;
        WasTap = false;
        WasSwipe = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isSwiping) return;
        _currentDragDelta = eventData.position - _startTouch;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isSwiping) return;

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
}
