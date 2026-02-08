using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyRootMotion : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform target;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private float chaseBuffer = 5.5f; // [NEW] Prevents jitter

    private NavMeshAgent _agent;
    private Animator _animator;
    private readonly int _speedHash = Animator.StringToHash("Speed");

    // Track if we are currently stopping to handle the buffer logic
    private bool _isStopped = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (target == null && GameObject.FindWithTag("Player"))
            target = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        // Decouple Agent from Transform so Root Motion can drive movement
        _agent.updatePosition = false;
        _agent.updateRotation = true;
    }

    private void Update()
    {
        if (target == null) return;

        // 1. Calculate Distance
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        // 2. Logic with Buffer (Hysteresis) to prevent Jitter
        if (_isStopped)
        {
            // If we are stopped, wait until player moves far enough (Stop Distance + Buffer)
            if (distanceToTarget > stopDistance + chaseBuffer)
            {
                _isStopped = false; // Start chasing
            }
        }
        else
        {
            // If we are chasing, stop when we get close
            if (distanceToTarget < stopDistance)
            {
                _isStopped = true; // Stop
            }
        }

        // 3. Move the Agent (Internally)
        if (!_isStopped)
        {
            _agent.SetDestination(target.position);
        }
        else
        {
            // Reset path so he doesn't try to push forward while stopped
            _agent.ResetPath();
        }

        // 4. Animate
        // Calculate the speed based on how the Agent *wants* to move
        float desiredSpeed = _isStopped ? 0f : Vector3.Dot(transform.forward, _agent.desiredVelocity);

        // Sanity check: prevent negative speed calculation if agent turns sharply
        desiredSpeed = Mathf.Max(0f, desiredSpeed);

        // Smoothly blend the animation (0.1f damp time)
        _animator.SetFloat(_speedHash, desiredSpeed, 0.1f, Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        // A. Apply Root Motion (XZ Movement)
        Vector3 position = _animator.rootPosition;

        // B. [FIX] Fix Slope/Height Issue
        // Override the Animation's Y height with the NavMeshAgent's Y height
        position.y = _agent.nextPosition.y;

        transform.position = position;

        // C. Sync the Agent to the new position so they stay together
        _agent.nextPosition = transform.position;
    }
}