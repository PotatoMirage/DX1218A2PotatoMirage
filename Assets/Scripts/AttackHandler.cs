using UnityEngine;
using Unity.Cinemachine;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private Collider[] detectors; // Assign your Sword SphereColliders here
    [SerializeField] private CinemachineImpulseSource[] source;

    // Safety check to prevent double-hitting the same enemy in one swing
    private bool _hasHit;

    private void Awake()
    {
        // Ensure all are disabled at start
        DisableCollider();
    }

    // Called by Animation Event (Start of active swing)
    public void EnableCollider(int index)
    {
        _hasHit = false; // Reset hit flag for new swing
        if (index < detectors.Length)
        {
            detectors[index].enabled = true;
        }
    }

    // Called by Animation Event (End of active swing)
    public void DisableCollider()
    {
        foreach (Collider detector in detectors)
        {
            detector.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasHit) return;

        // 2. Check Layer (Using CompareTag is often cheaper, but LayerMask is fine too)
        // Ensure your Enemy GameObject is on the "Target" layer!
        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Target")) != 0)
        {
            Debug.Log($"Hit {collision}!");

            _hasHit = true;

            // Generate Impulse (Shake)
            // Use index 0 for simplicity, or track which weapon is active
            if (source.Length > 0) source[0].GenerateImpulse(Camera.main.transform.forward);

            // Apply Damage logic here
            // e.g., other.GetComponent<Health>()?.TakeDamage(10);

            // Disable immediately if you only want 1 hit per swing
            DisableCollider();
        }
    }
}