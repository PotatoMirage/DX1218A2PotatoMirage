using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyRootMotion : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform target;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private float chaseBuffer = 5.5f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private readonly int _speedHash = Animator.StringToHash("Speed");
    private bool _isStopped = false;

    // Helper to check if we can move
    private bool IsAgentValid => _agent != null && _agent.isOnNavMesh && _agent.isActiveAndEnabled;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (target == null && GameObject.FindWithTag("Player"))
            target = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        if (IsAgentValid)
        {
            _agent.updatePosition = false;
            _agent.updateRotation = true;
        }
    }

    private void Update()
    {
        // SAFETY CHECK: If agent is disabled (dead), stop doing logic
        if (!IsAgentValid || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (_isStopped)
        {
            if (distanceToTarget > stopDistance + chaseBuffer) _isStopped = false;
        }
        else
        {
            if (distanceToTarget < stopDistance) _isStopped = true;
        }

        if (!_isStopped)
        {
            _agent.SetDestination(target.position);
        }
        else
        {
            _agent.ResetPath();
        }

        float desiredSpeed = _isStopped ? 0f : Vector3.Dot(transform.forward, _agent.desiredVelocity);
        desiredSpeed = Mathf.Max(0f, desiredSpeed);
        _animator.SetFloat(_speedHash, desiredSpeed, 0.1f, Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        // SAFETY CHECK here too
        if (!IsAgentValid) return;

        Vector3 position = _animator.rootPosition;
        position.y = _agent.nextPosition.y;
        transform.position = position;
        _agent.nextPosition = transform.position;
    }
}