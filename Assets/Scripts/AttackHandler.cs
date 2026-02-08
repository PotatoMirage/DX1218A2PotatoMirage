using UnityEngine;
using Unity.Cinemachine;

public class AttackHandler : MonoBehaviour
{
    [Header("Settings")]
    // [NEW] Select "Player" layer for Enemies, and "Target" layer for Player
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

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasHit) return;

        // [NEW] specific layer check using the LayerMask variable
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Debug.Log($"Hit {collision.gameObject.name}!");
            _hasHit = true;

            // 1. Camera Shake
            if (source.Length > 0 && source[0] != null)
                source[0].GenerateImpulse(Camera.main.transform.forward);

            // 2. Deal Damage
            // Check if we hit the Player
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // Add a TakeDamage method to your PlayerStats or PlayerController!
                Debug.Log("Dealt damage to Player");
                // player.Stats.TakeDamage(10); 
            }

            // Check if we hit an Enemy (reusing logic just in case)
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }

            DisableCollider();
        }
    }
}