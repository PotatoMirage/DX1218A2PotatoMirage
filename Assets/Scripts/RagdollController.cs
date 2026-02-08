using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; // Added for List

public class RagdollController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider rootCollider;
    [SerializeField] private Rigidbody rootRigidbody;

    private Rigidbody[] _boneRigidbodies;
    private Collider[] _boneColliders;

    private void Awake()
    {
        // 1. Get all Rigidbodies (These define the ragdoll bones)
        _boneRigidbodies = GetComponentsInChildren<Rigidbody>();

        // 2. Filter Colliders: Only get colliders that are attached to the Ragdoll Bones
        List<Collider> validColliders = new List<Collider>();

        foreach (var rb in _boneRigidbodies)
        {
            // Skip the main root object to avoid adding the Capsule Collider
            if (rb.gameObject == this.gameObject) continue;

            // Find all colliders on this specific bone
            Collider[] boneCols = rb.GetComponents<Collider>();
            validColliders.AddRange(boneCols);
        }

        _boneColliders = validColliders.ToArray();

        // 3. Start with Ragdoll Disabled
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
        if (rootRigidbody != null) rootRigidbody.isKinematic = !isRagdoll; // Fixed logic: Kinematic when Alive (true), Dynamic when Ragdoll (false)

        // C. Enable "Bone" Physics
        foreach (var rb in _boneRigidbodies)
        {
            if (rb.gameObject == this.gameObject) continue;
            rb.isKinematic = !isRagdoll;
        }

        foreach (var col in _boneColliders)
        {
            // We don't need the check here anymore because our list is already filtered
            col.enabled = isRagdoll;
        }
    }
}