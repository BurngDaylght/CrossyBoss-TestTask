using System.Collections;
using UnityEngine;
using Zenject;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _duration;
    [Range(0f, 100f)] [SerializeField] private float _shakingForce;
    [SerializeField] private AnimationCurve _curve;

    private LevelLogic _levelLogic;
    private PlayerStats _playerStats;
    
    [Inject]
    private void Construct(LevelLogic levelLogic, PlayerStats playerStats)
    {
        _levelLogic = levelLogic;
        _playerStats = playerStats;
    }

    private void OnEnable()
    {
        _levelLogic.OnLevelLosing += StartShake;
        _playerStats.OnPlayerDamaged += StartShake;
    }
    
    private void OnDisable()
    {
        _levelLogic.OnLevelLosing -= StartShake;
        _playerStats.OnPlayerDamaged -= StartShake;
    }

    private void StartShake()
    {
        StartCoroutine(Shaking());
    }

    private IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float strenght = _curve.Evaluate(elapsedTime / _duration);
            transform.position = startPosition + Random.insideUnitSphere * _shakingForce * strenght;
            yield return null;
        }

        transform.position = startPosition;
    }
}