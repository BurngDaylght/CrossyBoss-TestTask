using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class KeyDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image keyImage;
    
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
        if(_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        Lock lockArea = eventData.pointerEnter?.GetComponent<Lock>();
        if (lockArea != null && lockArea.CanAcceptKey(keyImage.color))
        {
            lockArea.AddKey(keyImage.color);
            gameObject.SetActive(false);
        }
        else
        {
            transform.SetParent(_originalParent);
            transform.SetSiblingIndex(_originalSiblingIndex);
            _rectTransform.DOAnchorPos(_startPos, 0.3f).SetEase(Ease.OutQuad);
        }
    }
}