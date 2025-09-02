using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 3f;
    [SerializeField] private float _currentHealth;

    public event Action OnPlayerDied;
    public event Action OnPlayerDamaged;

    public float CurrentHealth => _currentHealth;
    
    private HealthUI _healthUI;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _healthUI = GetComponentInChildren<HealthUI>();
    }

    private void Start()
    {
        _healthUI.Init(transform, _maxHealth);
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        _healthUI.UpdateHealth(_currentHealth);

        OnPlayerDamaged?.Invoke();

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _healthUI.Hide();
            OnPlayerDied?.Invoke();
        }
    }

    public bool IsAlive => _currentHealth > 0;
}
