using UnityEngine;
using Zenject;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject _joystick;

    private void Awake()
    {
        _joystick.SetActive(false);
    }

    private void OnEnable()
    {
        BattlePlatform.OnPlayerEnterBattleZone += ShowJoystick;
        BattlePlatform.OnPlayerExitBattleZone += HideJoystick;
    }

    private void OnDisable()
    {
        BattlePlatform.OnPlayerEnterBattleZone -= ShowJoystick;
        BattlePlatform.OnPlayerExitBattleZone -= HideJoystick;
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
