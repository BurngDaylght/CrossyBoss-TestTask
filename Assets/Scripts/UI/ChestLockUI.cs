using UnityEngine;
using DG.Tweening;
using Zenject;

public class ChestLockUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup _lockPanel;
    [SerializeField] private CanvasGroup _keyPanel;

    [Header("Animation Settings")]
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private Ease _ease = Ease.OutBack;

    private Vector3 _hiddenScale = Vector3.zero;
    private Vector3 _shownScale = Vector3.one;
    
    private ChestLock _chestLock;
    
    [Inject]
    private void Construct(ChestLock chestLock)
    {
        _chestLock = chestLock;
    }

    private void OnEnable()
    {
        _chestLock.OnLockCompleted += DisableRaycast;
    }

    private void OnDisable()
    {
        _chestLock.OnLockCompleted -= DisableRaycast;
    }
        

    private void Awake()
    {
        InitPanel(_lockPanel);
        InitPanel(_keyPanel);
    }

    private void InitPanel(CanvasGroup group)
    {
        group.alpha = 0f;
        group.transform.localScale = _hiddenScale;
    }

    public void Show(bool show)
    {
        AnimatePanel(_lockPanel, show);
        AnimatePanel(_keyPanel, show);
    }

    private void AnimatePanel(CanvasGroup group, bool show)
    {
        group.DOKill();
        group.transform.DOKill();

        if (show)
        {
            group.DOFade(1f, _duration * 0.6f);
            group.transform.DOScale(_shownScale, _duration)
                .SetEase(_ease);
        }
        else
        {
            group.DOFade(0f, _duration * 0.6f);
            group.transform.DOScale(_hiddenScale, _duration)
                .SetEase(Ease.InBack);
        }
    }
    
    private void DisableRaycast()
    {
        _keyPanel.interactable = false;
        _lockPanel.interactable = false;
    }

    private void OnDestroy()
    {
        _lockPanel.DOKill();
        _lockPanel.transform.DOKill();

        _keyPanel.DOKill();
        _keyPanel.transform.DOKill();
    }
}
