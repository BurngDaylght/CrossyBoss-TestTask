using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);

    private Vector3 _startScale;
    private Transform _target;
    private Camera _camera;

    private Tween _healthTween;

    private void Awake()
    {
        _startScale = transform.localScale;
    }

    public void Init(Transform target, float maxHealth)
    {
        _target = target;

        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = maxHealth;

        _camera = Camera.main;

        transform.localScale = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (_target == null || _camera == null) return;

        transform.position = _target.position + _offset;
        transform.LookAt(transform.position + _camera.transform.forward);
    }

    public void UpdateHealth(float currentHealth)
    {
        _healthTween?.Kill();

        _healthTween = DOTween.To(
            () => _healthSlider.value,
            x => _healthSlider.value = x,
            currentHealth,
            0.3f
        ).SetEase(Ease.OutCubic);
    }

    public void Show(float delay = 0f)
    {
        transform.DOScale(_startScale, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
    }

    public void Hide()
    {
        transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
    }
}
