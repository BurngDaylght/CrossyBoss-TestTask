using UnityEngine;
using Zenject;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float _swipeThreshold = 50f;

    private TouchField _touchField;
    private IMovable _movable;
    
    [Inject]
    private void Construct(TouchField touchField, IMovable movable)
    {
        _touchField = touchField;
        _movable = movable;
    }

    private void Update()
    {
        if (_touchField.WasTap)
        {
            _movable.MoveStraight();
            _touchField.ConsumeTap();
            return;
        }

        if (_touchField.WasSwipe)
        {
            Vector2 swipe = _touchField.SwipeDelta;
            float horizontal = swipe.x;
            float vertical = swipe.y;

            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                if (horizontal > _swipeThreshold) _movable.MoveRight();
                else if (horizontal < -_swipeThreshold) _movable.MoveLeft();
            }
            else
            {
                if (vertical > _swipeThreshold) _movable.MoveStraight();
                else if (vertical < -_swipeThreshold) _movable.MoveBack();
            }

            _touchField.ConsumeSwipe();
        }
    }
}
