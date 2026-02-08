using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private HealthComponent targetHealthComponent;

    private void OnEnable()
    {
        if (targetHealthComponent != null)
        {
            // Subscribe to the event
            targetHealthComponent.OnHealthPctChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (targetHealthComponent != null)
        {
            // Unsubscribe to prevent memory leaks
            targetHealthComponent.OnHealthPctChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(float pct)
    {
        // Update UI
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = pct;
        }
    }
}