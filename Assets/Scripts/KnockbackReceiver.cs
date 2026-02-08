using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// File: Assets/Scripts/Combat/KnockbackReceiver.cs
[RequireComponent(typeof(Rigidbody))]
public class KnockbackReceiver : MonoBehaviour, IKnockbackable
{
    [Header("Settings")]
    [SerializeField] private float dragOnGround = 5f;
    [SerializeField] private float dragInAir = 0f;
    [SerializeField] private float recoveryDelay = 0.5f;

    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private Coroutine _recoveryRoutine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();

        // Ensure Rigidbody is kinematic when controlled by Agent
        if (_agent != null) _rb.isKinematic = true;
    }

    public void ApplyKnockback(Vector3 force)
    {
        // 1. Switch control from Agent to Physics
        if (_agent != null)
        {
            _agent.enabled = false;
            _rb.isKinematic = false;
        }

        // 2. Apply Force
        _rb.linearDamping = dragInAir; // Reset drag for flight
        _rb.AddForce(force, ForceMode.Impulse);

        // 3. Start Recovery
        if (_recoveryRoutine != null) StopCoroutine(_recoveryRoutine);
        _recoveryRoutine = StartCoroutine(ResetControlRoutine());
    }

    private IEnumerator ResetControlRoutine()
    {
        yield return new WaitForSeconds(recoveryDelay);

        // Wait until almost stopped
        while (_rb.linearVelocity.magnitude > 0.5f)
        {
            yield return null;
        }

        // Return control to Agent
        if (_agent != null)
        {
            _rb.isKinematic = true;
            _agent.enabled = true;
            _agent.Warp(transform.position); // Sync agent to rb position
        }

        _rb.linearDamping = dragOnGround;
    }
}