using UnityEngine;
using UnityEngine.AI;

public enum EnemyBehaviorMode
{
    Passive,
    Sentinel,
    Aggressive
}

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyBehaviorMode enemyMode = EnemyBehaviorMode.Aggressive;

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int damageToPlayer = 10;

    [SerializeField] private Transform playerTarget;
    [SerializeField] private EnemyCombat combatSystem;

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
    }

    private void Update()
    {
        if (isDead || playerTarget == null || !agent.isActiveAndEnabled) return;

        switch (enemyMode)
        {
            case EnemyBehaviorMode.Passive:
                HandlePassiveState();
                break;

            case EnemyBehaviorMode.Sentinel:
                HandleCombatState(canChase: false);
                break;

            case EnemyBehaviorMode.Aggressive:
                HandleCombatState(canChase: true);
                break;
        }
    }

    private void HandlePassiveState()
    {
        if (!agent.isStopped) agent.isStopped = true;
        animator.SetFloat(SpeedHash, 0f);
    }

    private void HandleCombatState(bool canChase)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            animator.SetFloat(SpeedHash, 0f);

            RotateTowardsPlayer();

            if (Time.time >= lastAttackTime + attackCooldown && !combatSystem.IsAttacking)
            {
                combatSystem.StartAttackCombo();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            if (canChase)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTarget.position);
                animator.SetFloat(SpeedHash, agent.velocity.magnitude);
            }
            else
            {
                agent.isStopped = true;
                animator.SetFloat(SpeedHash, 0f);
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        animator.SetTrigger(DieHash);

        Destroy(gameObject, 5f);
    }
}