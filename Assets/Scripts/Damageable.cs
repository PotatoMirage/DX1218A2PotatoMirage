using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;

    // Observer: UI and PlayerController will listen to this
    public event UnityAction<float> OnHealthChanged = delegate { };
    public event UnityAction OnDeath = delegate { };
    public event UnityAction OnHit = delegate { };

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _stats.MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= amount;
        OnHealthChanged.Invoke(_currentHealth / _stats.MaxHealth);
        OnHit.Invoke();

        if (_currentHealth <= 0)
        {
            OnDeath.Invoke();
        }
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _stats.MaxHealth);
        OnHealthChanged.Invoke(_currentHealth / _stats.MaxHealth);
    }
}