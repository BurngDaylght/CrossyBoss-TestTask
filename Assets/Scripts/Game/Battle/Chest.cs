using System;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public static event Action OnChestInteracted;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            OnChestInteracted?.Invoke();
        }
    }
}
