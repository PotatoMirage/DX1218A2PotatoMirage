using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats stats;

    [SerializeField] private RagdollController ragdollController;

    private int currentHealth;

    public event Action<float> OnHealthPctChanged;
    public event Action OnDeath;

    private void Start()
    {
        if (stats != null) currentHealth = stats.MaxHealth;
        else currentHealth = 100;
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damageAmount;
        float currentPct = (float)currentHealth / (stats != null ? stats.MaxHealth : 100);
        OnHealthPctChanged?.Invoke(currentPct);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();

        if (ragdollController != null)
        {
            ragdollController.ActivateRagdoll();
        }

        EnemyRootMotion motion = GetComponent<EnemyRootMotion>();
        if (motion != null) motion.enabled = false;

        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null) controller.enabled = false;
    }
}