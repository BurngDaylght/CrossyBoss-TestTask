using UnityEngine;
using Zenject;
using DG.Tweening;

public class ChestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _chest;
    [SerializeField] private float _spawnRise = 0.5f;
    [SerializeField] private float _spawnDuration = 0.6f;
    [SerializeField] private float _spawnFromScale = 0.2f;

    private BattlePlatform _battlePlatform;
    private Vector3 _initialScale;
    private Vector3 _initialPosition;

    [Inject]
    private void Construct(BattlePlatform battlePlatform)
    {
        _battlePlatform = battlePlatform;
    }
    
    private void OnEnable() =>_battlePlatform.OnAllEnemiesDefeated += ShowChest;
    private void OnDisable() => _battlePlatform.OnAllEnemiesDefeated -= ShowChest;


    private void Awake()
    {
        _initialScale = _chest.transform.localScale;
        _initialPosition = _chest.transform.position;
        _chest.SetActive(false);
    }
    
    public void ShowChest()
    {
        _chest.SetActive(true);

        Transform chestTransform = _chest.transform;
        chestTransform.DOKill();

        chestTransform.position = _initialPosition - new Vector3(0f, _spawnRise, 0f);
        chestTransform.localScale = _initialScale * _spawnFromScale;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(chestTransform.DOMoveY(_initialPosition.y, _spawnDuration).SetEase(Ease.OutBack));
        sequence.Join(chestTransform.DOScale(_initialScale, _spawnDuration).SetEase(Ease.OutBack));
        sequence.Play();
    }
}
