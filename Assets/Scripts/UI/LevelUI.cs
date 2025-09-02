using UnityEngine;
using Zenject;

public class LevelUI : MonoBehaviour
{    
    [SerializeField] private CanvasGroup _lockUI;
    
    private StickUI _stick;
    private LevelLogic _levelLogic;   
    private BattlePlatform _battlePlatform;   
    private ChestLock _chestLock;   
    private Chest _chest;
    private ChestLockUI _chestLockUI;
    private CompleteLevelUI _completeLevelUI;
    private BattleStartTextUI _battleStartTextUI;
    
    [Inject]
    private void Construct(StickUI stick, LevelLogic levelLogic, BattlePlatform battlePlatform, ChestLock chestLock, Chest chest, ChestLockUI chestLockUI, CompleteLevelUI completeLevelUI, BattleStartTextUI battleStartTextUI)
    {
        _stick = stick;
        _levelLogic = levelLogic;
        _battlePlatform = battlePlatform;
        _chestLock = chestLock;
        _chest = chest;
        _chestLockUI = chestLockUI;
        _completeLevelUI = completeLevelUI;
        _battleStartTextUI = battleStartTextUI;
    }

    private void Awake()
    {
        HideLock();
    }

    private void OnEnable()
    {
        _battlePlatform.OnPlayerEnterBattleZone += ShowJoystick;
        _battlePlatform.OnPlayerEnterBattleZone += ShowBattleText;
        
        _chest.OnChestInteracted += ShowLock;

        _chestLock.OnLockCompleted += HideLock;

        _levelLogic.OnLevelComplete += ShowCompleteUI;
    }

    private void OnDisable()
    {
        _battlePlatform.OnPlayerEnterBattleZone -= ShowJoystick;
        _battlePlatform.OnPlayerEnterBattleZone -= ShowBattleText;
        
        _chest.OnChestInteracted -= ShowLock;
        
        _chestLock.OnLockCompleted -= HideLock;
        
        _levelLogic.OnLevelComplete -= ShowCompleteUI;
    }
    
    private void ShowLock()
    {
        _chestLockUI.Show(true);
    }
    
    private void HideLock()
    {
        _chestLockUI.Show(false);
    }
    
    private void ShowCompleteUI()
    {
        _completeLevelUI.Show();
    }

    private void ShowJoystick()
    {
        _stick.gameObject.SetActive(true);
        _stick.ShowStick(true);
    }
    
    private void ShowBattleText()
    {
        _battleStartTextUI.ShowBattleText();
    }
}
