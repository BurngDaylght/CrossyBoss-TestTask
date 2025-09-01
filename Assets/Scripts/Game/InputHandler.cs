using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputHandler : MonoBehaviour
{
    public event Action OnTap;
    [SerializeField] private float _swipeThreshold = 50f;
    
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset _inputAction;
    
    [Header("Action Map Name References")]
    [SerializeField] private string _actionMapName = "PlayerBattle";
    
    [Header("Action Name References")]
    [SerializeField] private string _move = "Move";
    
    private InputAction _moveAction;
    
    public Vector2 MoveInput { get; private set; }

    private TouchField _touchField;
    private IRoadMovable _movable;
    private IBattleMovable _battleMovable;
    
    private bool _firstTapOccurred = false;
    
    [Inject]
    private void Construct(TouchField touchField, IRoadMovable movable, IBattleMovable battleMovable)
    {
        _touchField = touchField;
        _movable = movable;
        _battleMovable = battleMovable;
    }
    
    private void Awake()
    {
        _moveAction = _inputAction.FindActionMap(_actionMapName).FindAction(_move);
        
        RegisterInputActions();
    }

    private void Update()
    {
        if (_touchField.WasTap)
        {
            OnTap?.Invoke();

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
        
        ReadMovement();
    }
    
    private void RegisterInputActions()
    {
        _moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        _moveAction.canceled += context => MoveInput = Vector2.zero;
    }
    
    private void ReadMovement()
    {
        _battleMovable.Move(MoveInput);
    }
    
    private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }
}
