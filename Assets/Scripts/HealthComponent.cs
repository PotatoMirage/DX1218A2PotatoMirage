using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [Header("Configuration")]
    [SerializeField] private PlayerStats stats;

    private int _currentHealth;

    // Observer Pattern: Events for UI or other systems to listen to
    public event Action<float> OnHealthPctChanged;
    public event Action OnDeath;

    private void Start()
    {
        // Initialize health from Scriptable Object config
        if (stats != null)
        {
            _currentHealth = stats.MaxHealth;
        }
        else
        {
            _currentHealth = 100;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damageAmount;

        // Notify Observers (Cleanly calculates percentage 0.0 to 1.0)
        float currentPct = (float)_currentHealth / stats.MaxHealth;
        OnHealthPctChanged?.Invoke(currentPct);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} has died.");
        // Handle death logic (ragdoll, disable controller, etc.)
    }
}