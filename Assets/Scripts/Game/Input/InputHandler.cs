using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputHandler : MonoBehaviour
{
    [Header("Swipe Settings")]
    [SerializeField] private float _swipeThreshold = 50f;

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset _inputAction;

    [Header("Action Map & Action Names")]
    [SerializeField] private string _actionMapName = "PlayerBattle";
    [SerializeField] private string _moveActionName = "Move";

    private InputAction _moveAction;
    public Vector2 MoveInput { get; private set; }

    private TouchField _touchField;
    private IRoadMovable _roadMovable;
    private IBattleMovable _battleMovable;

    private bool _firstTapOccurred = false;

    [Inject]
    private void Construct(TouchField touchField, IRoadMovable roadMovable, IBattleMovable battleMovable)
    {
        _touchField = touchField;
        _roadMovable = roadMovable;
        _battleMovable = battleMovable;
    }

    private void Awake()
    {
        _moveAction = _inputAction.FindActionMap(_actionMapName).FindAction(_moveActionName);
        RegisterInputActions();
    }

    private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }

    private void Update()
    {
        HandleTapAndSwipe();
        ReadBattleMovement();
    }

    private void HandleTapAndSwipe()
    {
        if (_touchField.WasTap)
        {
            OnTap?.Invoke();
            _roadMovable.MoveStraight();
            _touchField.ConsumeTap();
            return;
        }

        if (_touchField.WasSwipe)
        {
            OnTap?.Invoke();
            Vector2 swipe = _touchField.SwipeDelta;
            float horizontal = swipe.x;
            float vertical = swipe.y;

            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                if (horizontal > _swipeThreshold) _roadMovable.MoveRight();
                else if (horizontal < -_swipeThreshold) _roadMovable.MoveLeft();
            }
            else
            {
                if (vertical > _swipeThreshold) _roadMovable.MoveStraight();
                else if (vertical < -_swipeThreshold) _roadMovable.MoveBack();
            }

            _touchField.ConsumeSwipe();
        }
    }

    private void RegisterInputActions()
    {
        _moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        _moveAction.canceled += context => MoveInput = Vector2.zero;
    }

    private void ReadBattleMovement()
    {
        _battleMovable.Move(MoveInput);
    }

    public event Action OnTap;
}
