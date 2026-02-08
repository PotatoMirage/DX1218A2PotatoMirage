using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    [SerializeField] private HealthComponent targetHealthComponent;

    private void OnEnable()
    {
        if (targetHealthComponent != null)
        {
            targetHealthComponent.OnHealthPctChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (targetHealthComponent != null)
        {
            targetHealthComponent.OnHealthPctChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(float pct)
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = pct;
        }
    }
}