using UnityEngine;
using Zenject;

public class LevelUI : MonoBehaviour
{    
    [SerializeField] private CanvasGroup _lockUI;
    [SerializeField] private GameObject _joystick;
    
    private LevelLogic _levelLogic;   
    private BattlePlatform _battlePlatform;   
    private ChestLock _chestLock;   
    private Chest _chest;
    
    [Inject]
    private void Construct(LevelLogic levelLogic, BattlePlatform battlePlatform, ChestLock chestLock, Chest chest)
    {
        _levelLogic = levelLogic;
        _battlePlatform = battlePlatform;
        _chestLock = chestLock;
        _chest = chest;
    }

    private void Awake()
    {
        HideJoystick();
        HideLock();
    }

    private void OnEnable()
    {
        _battlePlatform.OnPlayerEnterBattleZone += ShowJoystick;
        _battlePlatform.OnPlayerExitBattleZone += HideJoystick;
        
        _chest.OnChestInteracted += ShowLock;

        _chestLock.OnLockCompleted += HideLock;

        _levelLogic.OnLevelComplete += HideJoystick;
    }

    private void OnDisable()
    {
        _battlePlatform.OnPlayerEnterBattleZone -= ShowJoystick;
        _battlePlatform.OnPlayerExitBattleZone -= HideJoystick;
        
        _chest.OnChestInteracted -= ShowLock;
        
        _chestLock.OnLockCompleted -= HideLock;
        
        _levelLogic.OnLevelComplete -= HideJoystick;
    }
    
    private void ShowLock()
    {
        _lockUI.alpha = 1f;
        _lockUI.interactable = true;
        _lockUI.blocksRaycasts = true;
    }
    
    private void HideLock()
    {
        _lockUI.alpha = 0f;
        _lockUI.interactable = false;
        _lockUI.blocksRaycasts = false;
    }

    private void ShowJoystick()
    {
        _joystick.SetActive(true);
    }

    private void HideJoystick()
    {
        _joystick.SetActive(false);
    }
}
