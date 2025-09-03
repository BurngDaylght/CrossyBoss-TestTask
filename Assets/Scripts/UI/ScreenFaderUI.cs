using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Zenject;

[RequireComponent(typeof(CanvasGroup), typeof(Image))]
public class ScreenFaderUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private Ease _fadeEase = Ease.Linear;

    private CanvasGroup _canvasGroup;
    private Image _image;

    private bool _isFading = false;

    private LevelLogic _levelLogic;

    [Inject]
    private void Construct(LevelLogic levelLogic)
    {
        _levelLogic = levelLogic;
    }

    private void OnEnable() => _levelLogic.OnLevelRestart += FadeIn;

    private void OnDisable() => _levelLogic.OnLevelRestart -= FadeIn;
    
    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();

        if (_image != null)
            _image.color = Color.black;

        _canvasGroup.blocksRaycasts = false;

        FadeOut();
    }

    private void OnDestroy()
    {
        if (_canvasGroup != null)
            _canvasGroup.DOKill();
    }

    public void FadeIn()
    {
        if (_isFading) return;
        _isFading = true;

        _canvasGroup.DOKill();
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1f, _fadeDuration)
            .SetEase(_fadeEase)
            .OnComplete(() => _isFading = false);
    }

    public void FadeOut()
    {
        if (_isFading) return;
        _isFading = true;

        _canvasGroup.DOKill();
        _canvasGroup.DOFade(0f, _fadeDuration)
            .SetEase(_fadeEase)
            .OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = false;
                _isFading = false;
            });
    }

    public void SetTransparent()
    {
        _canvasGroup.DOKill();
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _isFading = false;
    }

    public void SetOpaque()
    {
        _canvasGroup.DOKill();
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _isFading = false;
    }
}
