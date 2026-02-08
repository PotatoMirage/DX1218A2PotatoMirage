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
    [Tooltip("Prefab to instantiate on hit (e.g., blood spatter, sparks)")]
    [SerializeField] private GameObject hitEffectPrefab;
    [Tooltip("Sound to play on successful hit")]
    [SerializeField] private AudioClip hitSound;
    [Tooltip("Optional: specific source to play sound. If null, will use PlayClipAtPoint")]
    [SerializeField] private AudioSource audioSource;

    private bool _hasHit;

    private void Awake()
    {
        DisableCollider();
        // Try to get AudioSource if not assigned
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
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

        // Check layer match
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            _hasHit = true;

            // --- 1. Visual Effect ---
            if (hitEffectPrefab != null)
            {
                // Find the point of contact on the other collider closest to our weapon
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Instantiate(hitEffectPrefab, hitPoint, Quaternion.LookRotation(transform.forward));
            }

            // --- 2. Sound Effect ---
            if (hitSound != null)
            {
                if (audioSource != null && audioSource.isActiveAndEnabled)
                {
                    audioSource.PlayOneShot(hitSound);
                }
                else
                {
                    // Fallback if no AudioSource is attached
                    AudioSource.PlayClipAtPoint(hitSound, transform.position);
                }
            }

            // --- 3. Camera Shake ---
            if (source.Length > 0 && source[0] != null)
                source[0].GenerateImpulse(Camera.main.transform.forward);

            // --- 4. Deal Damage ---
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.OnTakeHit(10);
            }

            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }

            DisableCollider();
        }
    }
}