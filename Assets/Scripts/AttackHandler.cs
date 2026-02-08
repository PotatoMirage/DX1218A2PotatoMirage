using UnityEngine;
using Unity.Cinemachine;

public class AttackHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask targetLayer;

    [Header("References")]
    [SerializeField] private Collider[] detectors;
    [SerializeField] private CinemachineImpulseSource[] source;

    private bool _hasHit;

    private void Awake()
    {
        DisableCollider();
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

    // [CHANGED] From OnCollisionEnter to OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        // [CHANGED] Logic to check layer on 'other.gameObject'
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Debug.Log($"Hit {other.name}!");
            _hasHit = true;

            // 1. Camera Shake
            if (source.Length > 0 && source[0] != null)
                source[0].GenerateImpulse(Camera.main.transform.forward);

            // 2. Deal Damage
            // Check for Player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // player.Stats.TakeDamage(10); 
            }

            // Check for Enemy
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }

            // Optional: Disable immediately if you only want 1 hit per swing
            DisableCollider();
        }
    }
    
}