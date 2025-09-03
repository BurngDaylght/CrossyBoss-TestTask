using UnityEngine;
using DG.Tweening;
using Zenject;

public class CompleteLevelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup _endText;
    [SerializeField] private UIButton _restartButton;

    [Header("Animation Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _scaleDuration = 0.5f;

    private LevelLogic _levelLogic;

    [Inject]
    private void Construct(LevelLogic levelLogic)
    {
        _levelLogic = levelLogic;
    }

    private void OnEnable() => _restartButton.OnClick += _levelLogic.RestartLevel;

    private void OnDisable() => _restartButton.OnClick -= _levelLogic.RestartLevel;
    
    private void Start()
    {
        SetImmediateHide();
    }

    private void OnDestroy()
    {
        _endText?.DOKill();
        _endText?.transform.DOKill();
        _restartButton?.DOKill();
    }

    public void SetImmediateHide()
    {
        if (_endText != null)
        {
            _endText.DOKill();
            _endText.alpha = 0f;
            _endText.interactable = false;
            _endText.blocksRaycasts = false;
        }

        if (_restartButton != null)
        {
            _restartButton.Hide(true);
        }
    }

    public void FadeOut()
    {
        if (_endText != null)
        {
            _endText.DOKill();
            _endText.DOFade(0f, _fadeDuration).OnComplete(() =>
            {
                _endText.interactable = false;
                _endText.blocksRaycasts = false;
            });
        }

        if (_restartButton != null)
        {
            _restartButton.Hide();
        }
    }

    public void Show()
    {
        if (_endText != null)
        {
            _endText.DOKill();
            _endText.alpha = 0f;
            _endText.transform.localScale = Vector3.zero;
            _endText.interactable = true;
            _endText.blocksRaycasts = true;

            _endText.DOFade(1f, _fadeDuration);
            _endText.transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack);
        }

        if (_restartButton != null)
        {
            _restartButton.Show();
        }
    }
}
