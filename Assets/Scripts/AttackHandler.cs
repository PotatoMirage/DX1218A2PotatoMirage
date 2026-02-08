using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(AudioSource))]
public class AttackHandler : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private Collider[] detectors;
    [SerializeField] private CinemachineImpulseSource[] source;

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;

    private bool hasHit;
    private int currentDamage = 10;

    private void Awake()
    {
        DisableCollider();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void SetupAttack(AttackConfigSO config)
    {
        currentDamage = config != null ? config.damageAmount : 10;
    }

    public void EnableCollider(int index)
    {
        hasHit = false;
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
        if (hasHit) return;

        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            hasHit = true;

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

            IDamageable damageable = other.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(currentDamage);
            }
            else
            {
                PlayerController player = other.GetComponentInParent<PlayerController>();
                if (player != null) player.OnTakeHit(currentDamage);
            }

            DisableCollider();
        }
    }
}