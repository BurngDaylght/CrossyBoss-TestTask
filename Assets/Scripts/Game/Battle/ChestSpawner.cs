using UnityEngine;
using Zenject;

public class ChestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _chest;

    private BattlePlatform _battlePlatform;
    
    [Inject]
    private void Construct(BattlePlatform battlePlatform)
    {
        _battlePlatform = battlePlatform;
    }

    private void OnEnable()
    {
        _battlePlatform.OnAllEnemiesDefeated += ShowChest;
    }

    private void OnDisable()
    {
        _battlePlatform.OnAllEnemiesDefeated -= ShowChest;
    }

    public void ShowChest()
    {
        _chest.SetActive(true);
    }
}
