using UnityEngine;
using UnityEngine.AI;

public class RagdollController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider rootCollider; // The Capsule Collider on the main object
    [SerializeField] private Rigidbody rootRigidbody; // The Rigidbody on the main object

    private Rigidbody[] _boneRigidbodies;
    private Collider[] _boneColliders;

    private void Awake()
    {
        // 1. Get all components in children
        _boneRigidbodies = GetComponentsInChildren<Rigidbody>();
        _boneColliders = GetComponentsInChildren<Collider>();

        // 2. Start with Ragdoll Disabled
        ToggleRagdoll(false);
    }

    public void ActivateRagdoll()
    {
        ToggleRagdoll(true);
    }

    private void ToggleRagdoll(bool isRagdoll)
    {
        // A. Disable "Alive" Components
        if (animator != null) animator.enabled = !isRagdoll;
        if (agent != null) agent.enabled = !isRagdoll;

        // B. Disable Root Physics so it doesn't fall through the floor
        if (rootCollider != null) rootCollider.enabled = !isRagdoll;
        if (rootRigidbody != null) rootRigidbody.isKinematic = true;

        // C. Enable "Bone" Physics
        foreach (var rb in _boneRigidbodies)
        {
            // Skip the root object itself
            if (rb.gameObject == this.gameObject) continue;
            rb.isKinematic = !isRagdoll; // Physics ON when ragdoll is ON
        }

        foreach (var col in _boneColliders)
        {
            // Skip the root object itself
            if (col.gameObject == this.gameObject) continue;
            col.enabled = isRagdoll;
        }
    }
}