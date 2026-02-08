using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyRootMotion : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform target;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private float chaseBuffer = 5.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private readonly int speedHash = Animator.StringToHash("Speed");
    private bool isStopped = false;

    private bool IsAgentValid => agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (target == null && GameObject.FindWithTag("Player"))
            target = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        if (IsAgentValid)
        {
            agent.updatePosition = false;
            agent.updateRotation = true;
        }
    }

    private void Update()
    {
        if (!IsAgentValid || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (isStopped)
        {
            if (distanceToTarget > stopDistance + chaseBuffer) isStopped = false;
        }
        else
        {
            if (distanceToTarget < stopDistance) isStopped = true;
        }

        if (!isStopped)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.ResetPath();
        }

        float desiredSpeed = isStopped ? 0f : Vector3.Dot(transform.forward, agent.desiredVelocity);
        desiredSpeed = Mathf.Max(0f, desiredSpeed);
        animator.SetFloat(speedHash, desiredSpeed, 0.1f, Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        if (!IsAgentValid) return;

        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}