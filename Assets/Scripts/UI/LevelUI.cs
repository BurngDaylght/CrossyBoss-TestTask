using UnityEngine;
using Zenject;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _lockUI;
    [SerializeField] private GameObject _joystick;

    private void Awake()
    {
        HideJoystick();
        HideLock();
    }

    private void OnEnable()
    {
        BattlePlatform.OnPlayerEnterBattleZone += ShowJoystick;
        BattlePlatform.OnPlayerExitBattleZone += HideJoystick;
        
        Chest.OnChestInteracted += ShowLock;

        Lock.OnLockCompleted += HideLock;
    }

    private void OnDisable()
    {
        BattlePlatform.OnPlayerEnterBattleZone -= ShowJoystick;
        BattlePlatform.OnPlayerExitBattleZone -= HideJoystick;
        
        Chest.OnChestInteracted -= ShowLock;
        
        Lock.OnLockCompleted -= HideLock;
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
