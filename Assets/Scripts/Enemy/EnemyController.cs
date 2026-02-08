using UnityEngine;
using UnityEngine.AI;

// Requires a NavMeshAgent to move and an Animator for visuals
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
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
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Auto-find player if not assigned
        if (playerTarget == null)
            playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (_isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // 1. Movement Logic
        if (distanceToPlayer <= attackRange)
        {
            // Stop moving if in range
            _agent.isStopped = true;
            _animator.SetFloat(SpeedHash, 0f);

            // Rotate to face player
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0; // Keep rotation flat
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            if (Time.time >= _lastAttackTime + attackCooldown && !combatSystem.IsAttacking)
            {
                // Stop moving while attacking so we don't slide
                _agent.isStopped = true;

                // Start the combo
                combatSystem.StartAttackCombo();

                _lastAttackTime = Time.time;
            }
        }
        else
        {
            // Chase the player
            _agent.isStopped = false;
            _agent.SetDestination(playerTarget.position);
            _animator.SetFloat(SpeedHash, _agent.velocity.magnitude);
        }
    }

    // Call this from your AttackHandler.cs
    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        currentHealth -= amount;

        // Optional: Play hit reaction animation here
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
        _agent.enabled = false; // Disable navigation
        GetComponent<Collider>().enabled = false; // Disable collision so player walks through

        _animator.SetTrigger(DieHash);

        // Optional: If you want to use your RagdollController, enable it here
        // GetComponent<RagdollController>()?.ActivateRagdoll(true);

        Destroy(gameObject, 5f); // Clean up body after 5 seconds
    }
}