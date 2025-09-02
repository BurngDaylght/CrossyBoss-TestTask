using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class KeyDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public Image keyImage;

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease moveEase = Ease.InOutQuad;
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private Ease scaleEase = Ease.InQuad;
    [SerializeField] private float dragScaleFactor = 1.5f;
    [SerializeField] private float dragRotationAngle = 45f;

    private Vector2 _startPos;
    private Transform _originalParent;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private int _originalSiblingIndex;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPos = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        _originalSiblingIndex = transform.GetSiblingIndex();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        _rectTransform.DOKill();

        RectTransform canvasRect = _canvas.transform as RectTransform;
        Vector3 worldPos = transform.position;
        transform.SetParent(_canvas.transform, true);
        Vector2 anchoredInCanvas = canvasRect.InverseTransformPoint(worldPos);
        _rectTransform.anchoredPosition = anchoredInCanvas;

        transform.SetAsLastSibling();

        _rectTransform.DOScale(dragScaleFactor, 0.2f).SetEase(Ease.OutBack);
        _rectTransform.DORotate(new Vector3(0, 0, dragRotationAngle), 0.2f).SetEase(Ease.OutBack);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        
        ChestLock lockArea = eventData.pointerEnter?.GetComponentInParent<ChestLock>();
        if (lockArea != null)
        {
            if (lockArea.CanAcceptKey(keyImage.sprite))
            {
                AnimateKeyToLock(lockArea);
                return;
            }
            else
            {
                lockArea.RejectKeyFeedback();

                _rectTransform.DOKill();
                _rectTransform.DOPunchScale(Vector3.one * 0.12f, 0.25f, 8, 0.5f);
            }
        }

        _rectTransform.DOKill();

        RectTransform canvasRect = _canvas.transform as RectTransform;
        Vector3 worldTarget = (_originalParent as RectTransform).TransformPoint(_startPos);
        Vector2 targetAnchoredInCanvas = canvasRect.InverseTransformPoint(worldTarget);

        _rectTransform.DOAnchorPos(targetAnchoredInCanvas, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.SetParent(_originalParent, false);
                _rectTransform.anchoredPosition = _startPos;
                _rectTransform.localScale = Vector3.one;
                _rectTransform.localEulerAngles = Vector3.zero;
                transform.SetSiblingIndex(_originalSiblingIndex);
            });

        _rectTransform.DOScale(1f, 0.2f).SetEase(Ease.InOutQuad);
        _rectTransform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.InOutQuad);
    }

    private void AnimateKeyToLock(ChestLock lockArea)
    {
        _rectTransform.DOKill();

        Vector3 targetPos = lockArea.transform.position;
        _rectTransform.SetParent(_canvas.transform, true);

        _rectTransform.DOMove(targetPos, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {
                lockArea.AddKey(keyImage.sprite);
                gameObject.SetActive(false);
            });

        _rectTransform.DOScale(0f, scaleDuration).SetEase(scaleEase);
    }
}
