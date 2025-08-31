using System;
using UnityEngine;

public class BattlePlatform : MonoBehaviour
{
    [Header("Battle Settings")]
    [SerializeField] private float _zoneSize = 1f;
    [SerializeField] private Vector3 _zoneCenter = Vector3.zero;
    [SerializeField] private bool _useTrigger = true;
    
    public static event Action OnPlayerEnterBattleZone;
    public static event Action OnPlayerExitBattleZone;

    private SphereCollider _collider;

    private void OnValidate()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = _useTrigger;
        _collider.radius = _zoneSize;
        _collider.center = _zoneCenter;
    }

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = _useTrigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement controller))
        {
            OnPlayerEnterBattleZone?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement controller))
        {
            OnPlayerExitBattleZone?.Invoke();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawSphere(_zoneCenter, _zoneSize);
    }
}
