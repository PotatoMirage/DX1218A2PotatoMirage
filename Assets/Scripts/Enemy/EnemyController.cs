using UnityEngine;
using UnityEngine.AI;

public enum EnemyBehaviorMode
{
    Passive,    // Dummy: Stands still, does nothing
    Sentinel,   // Guard: Stands still, but attacks if player is within range
    Aggressive  // Hunter: Chases player and attacks
}

// Requires a NavMeshAgent to move and an Animator for visuals
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("AI Settings")]
    [Tooltip("Passive = Dummy\nSentinel = Static Attacker\nAggressive = Chaser")]
    [SerializeField] private EnemyBehaviorMode enemyMode = EnemyBehaviorMode.Aggressive;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int damageToPlayer = 10;

    [Header("References")]
    [SerializeField] private Transform playerTarget; // Drag your Player object here
    [SerializeField] private EnemyCombat combatSystem;

    // Internal State
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _lastAttackTime;
    private bool _isDead = false;

    // Animator Parameter Hashes for performance
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Auto-find player if not assigned
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
    }

    private void Update()
    {
        if (_isDead || playerTarget == null) return;

        // State Machine
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
        // Force stop and idle animation
        if (!_agent.isStopped) _agent.isStopped = true;
        _animator.SetFloat(SpeedHash, 0f);
    }

    private void HandleCombatState(bool canChase)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            // --- IN RANGE: Stop and Attack ---
            _agent.isStopped = true;
            _animator.SetFloat(SpeedHash, 0f);

            RotateTowardsPlayer();

            // Attack Logic
            if (Time.time >= _lastAttackTime + attackCooldown && !combatSystem.IsAttacking)
            {
                combatSystem.StartAttackCombo();
                _lastAttackTime = Time.time;
            }
        }
        else
        {
            // --- OUT OF RANGE ---
            if (canChase)
            {
                // Aggressive: Move to player
                _agent.isStopped = false;
                _agent.SetDestination(playerTarget.position);
                _animator.SetFloat(SpeedHash, _agent.velocity.magnitude);
            }
            else
            {
                // Sentinel: Just stand still and wait
                _agent.isStopped = true;
                _animator.SetFloat(SpeedHash, 0f);
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0; // Keep rotation flat
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    // Call this from your AttackHandler.cs
    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        currentHealth -= amount;

        // Optional: Play hit reaction here
        // _animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        _agent.isStopped = true;
        _agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        _animator.SetTrigger(DieHash);

        Destroy(gameObject, 5f);
    }
}