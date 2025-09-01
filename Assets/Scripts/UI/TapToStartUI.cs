using UnityEngine;
using TMPro;
using DG.Tweening;
using Zenject;

public class TapToStartUI : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _pulseScale = 1.1f;
    [SerializeField] private float _pulseDuration = 0.8f;

    private TextMeshProUGUI _tapToStartText;
    private Tween _idleTween;
    
    private InputHandler _inputHandler;
    private LevelLogic _levelLogic;

    [Inject]
    private void Construct(LevelLogic levelLogic, InputHandler inputHandler)
    {
        _levelLogic = levelLogic;
        _inputHandler = inputHandler;
    }
    
    private void Awake()
    {
        _tapToStartText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _inputHandler.OnTap += HandleLevelStart;
    }
    
    private void Start()
    {
        PlayIdleAnimation();
    }

    private void HandleLevelStart()
    {
        _inputHandler.OnTap -= HandleLevelStart;
        
        _idleTween?.Kill();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(_tapToStartText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack));
        sequence.Append(_tapToStartText.DOFade(0f, _fadeDuration));
        sequence.Join(_tapToStartText.transform.DOScale(0f, _fadeDuration));
        sequence.OnComplete(() => gameObject.SetActive(false));
    }

    private void PlayIdleAnimation()
    {
        _idleTween = _tapToStartText.transform.DOScale(_pulseScale, _pulseDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    
    private void OnDestroy()
    {
        transform.DOKill();

        _tapToStartText.DOKill();
        _tapToStartText.transform.DOKill();

        _idleTween?.Kill();
    }
}
