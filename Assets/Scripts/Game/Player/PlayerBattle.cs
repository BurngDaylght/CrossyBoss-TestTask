using UnityEngine;
using Zenject;

public class PlayerBattle : MonoBehaviour, IBattleMovable
{
    [SerializeField] private float _moveSpeed = 5f;
    
    private bool _inBattle = false;
    private bool _controlEnabled = false;
    
    private PlayerAnimation _playerAnimation;
    
    private BattlePlatform[] _battlePlatforms;
    
    [Inject]
    public void Construct(BattlePlatform[] battlePlatforms)
    {
        _battlePlatforms = battlePlatforms;
    }
    
    private void OnEnable()
    {
        BattlePlatform.OnPlayerEnterBattleZone += HandleEnterBattleZone;
        BattlePlatform.OnPlayerExitBattleZone += HandleExitBattleZone;
    }

    private void OnDisable()
    {
        BattlePlatform.OnPlayerEnterBattleZone -= HandleEnterBattleZone;
        BattlePlatform.OnPlayerExitBattleZone -= HandleExitBattleZone;
    }
    
    private void Awake()
    {
        _playerAnimation = GetComponent<PlayerAnimation>();
    }
    
    public void EnterBattle()
    {
        _inBattle = true;
        Debug.Log("Игрок вошёл в боевой режим!");
    }
    
    public void Move(Vector2 direction)
    {
        if (!_inBattle || !_controlEnabled) return;
        
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y).normalized;
        transform.position += moveDirection * _moveSpeed * Time.deltaTime;
        
        _playerAnimation?.PlayMoveAnimation(moveDirection);
    }
    
    private void HandleEnterBattleZone()
    {
        EnterBattle();
        EnableControl(true);
    }

    private void HandleExitBattleZone()
    {
        EnableControl(false);
    }

    public void EnableControl(bool enabled)
    {
        _controlEnabled = enabled;
    }
}
