using System;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public event Action OnChestInteracted;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            OnChestInteracted?.Invoke();
        }
    }
}
