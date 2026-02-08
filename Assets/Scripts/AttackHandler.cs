using UnityEngine;
using Unity.Cinemachine;

public class AttackHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask targetLayer;

    [Header("References")]
    [SerializeField] private Collider[] detectors;
    [SerializeField] private CinemachineImpulseSource[] source;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;

    private bool _hasHit;
    // Store damage amount if you want it variable, otherwise use a default
    private int _currentDamage = 10;

    private void Awake()
    {
        DisableCollider();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void SetupAttack(AttackConfigSO config)
    {
        // If config exists, use its damage, otherwise default to 10
        _currentDamage = config != null ? config.damageAmount : 10;
    }

    public void EnableCollider(int index)
    {
        _hasHit = false;
        if (index < detectors.Length && detectors[index] != null)
        {
            detectors[index].enabled = true;
        }
    }

    public void DisableCollider()
    {
        foreach (Collider detector in detectors)
        {
            if (detector != null) detector.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            _hasHit = true;

            // --- 1. Visual & Audio Effects ---
            if (hitEffectPrefab != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Instantiate(hitEffectPrefab, hitPoint, Quaternion.LookRotation(transform.forward));
            }

            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            if (source.Length > 0 && source[0] != null)
                source[0].GenerateImpulse(Camera.main.transform.forward);

            // --- 2. Deal Damage ONLY (No Knockback) ---

            // Search on the object AND its parents (Fixes hitting child colliders)
            IDamageable damageable = other.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(_currentDamage);
            }
            // Fallback for direct references if needed
            else
            {
                PlayerController player = other.GetComponentInParent<PlayerController>();
                if (player != null) player.OnTakeHit(_currentDamage);
            }

            DisableCollider();
        }
    }
}