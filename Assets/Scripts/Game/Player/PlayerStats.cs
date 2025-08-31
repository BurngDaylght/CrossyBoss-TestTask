using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _currentHealth;

    public event Action OnPlayerDied;
    public float CurrentHealth => _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        Debug.Log($"Игрок получил {amount} урона. HP: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnPlayerDied?.Invoke();
            Debug.Log("Игрок погиб!");
        }
    }

    public bool IsAlive => _currentHealth > 0;
}
