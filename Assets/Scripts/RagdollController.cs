using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider rootCollider;
    [SerializeField] private Rigidbody rootRigidbody;

    private Rigidbody[] _boneRigidbodies;
    private Collider[] _boneColliders;

    private void Awake()
    {
        _boneRigidbodies = GetComponentsInChildren<Rigidbody>();

        List<Collider> validColliders = new();

        foreach (Rigidbody rb in _boneRigidbodies)
        {
            if (rb.gameObject == this.gameObject) continue;

            Collider[] boneCols = rb.GetComponents<Collider>();
            validColliders.AddRange(boneCols);
        }

        _boneColliders = validColliders.ToArray();

        ToggleRagdoll(false);
    }

    public void ActivateRagdoll()
    {
        ToggleRagdoll(true);
    }

    private void ToggleRagdoll(bool isRagdoll)
    {
        if (animator != null) animator.enabled = !isRagdoll;
        if (agent != null) agent.enabled = !isRagdoll;

        if (rootCollider != null) rootCollider.enabled = !isRagdoll;
        if (rootRigidbody != null) rootRigidbody.isKinematic = !isRagdoll;

        foreach (Rigidbody rb in _boneRigidbodies)
        {
            if (rb.gameObject == this.gameObject) continue;
            rb.isKinematic = !isRagdoll;
        }

        foreach (Collider col in _boneColliders)
        {
            col.enabled = isRagdoll;
        }
    }
}