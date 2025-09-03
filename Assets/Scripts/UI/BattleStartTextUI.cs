using UnityEngine;
using TMPro;
using DG.Tweening;

public class BattleStartTextUI : MonoBehaviour
{
    [SerializeField] private float _showDuration = 0.5f;
    [SerializeField] private float _stayDuration = 1.5f;
    [SerializeField] private float _hideDuration = 0.5f;
    [SerializeField] private float _punchScale = 1.2f;
    
    private TextMeshProUGUI _battleText;
    
    private void Start()
    {
        _battleText = GetComponent<TextMeshProUGUI>();
        _battleText.alpha = 0f;
        _battleText.transform.localScale = Vector3.one;
    }

    public void ShowBattleText(string text = "BATTLE!")
    {
        _battleText.text = text;
        _battleText.DOKill();
        _battleText.alpha = 0f;
        _battleText.transform.localScale = Vector3.one;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_battleText.DOFade(1f, _showDuration));
        sequence.Join(_battleText.transform.DOScale(Vector3.one * _punchScale, _showDuration).SetEase(Ease.OutBack));

        sequence.AppendInterval(_stayDuration);

        sequence.Append(_battleText.DOFade(0f, _hideDuration));
        sequence.Join(_battleText.transform.DOScale(Vector3.one, _hideDuration));

        sequence.Play();
    }
}
