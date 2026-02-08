using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [Header("Configuration")]
    [SerializeField] private PlayerStats stats;

    // LINK TO RAGDOLL
    [Header("References")]
    [SerializeField] private RagdollController ragdollController;

    private int _currentHealth;

    public event Action<float> OnHealthPctChanged;
    public event Action OnDeath;

    private void Start()
    {
        if (stats != null) _currentHealth = stats.MaxHealth;
        else _currentHealth = 100;
    }

    public void TakeDamage(int damageAmount)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damageAmount;
        float currentPct = (float)_currentHealth / (stats != null ? stats.MaxHealth : 100);
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

        // TRIGGER RAGDOLL
        if (ragdollController != null)
        {
            ragdollController.ActivateRagdoll();
        }

        // Disable logic scripts so they don't interfere
        EnemyRootMotion motion = GetComponent<EnemyRootMotion>();
        if (motion != null) motion.enabled = false;

        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null) controller.enabled = false;
    }
}