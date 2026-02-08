using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyCombat : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<AttackConfigSO> comboChain;
    [SerializeField] private float damage = 10f;

    private Animator _animator;
    private bool _isAttacking;

    // Helper to let the AI know when we are busy
    public bool IsAttacking => _isAttacking;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Called by your EnemyController (AI)
    public void StartAttackCombo()
    {
        if (_isAttacking) return;

        StartCoroutine(PerformComboRoutine());
    }

    private IEnumerator PerformComboRoutine()
    {
        _isAttacking = true;
        _animator.SetBool("IsAttack", true);

        // Loop through every attack in the SO list automatically
        for (int i = 0; i < comboChain.Count; i++)
        {
            AttackConfigSO attack = comboChain[i];

            // 1. Trigger Animation
            _animator.SetInteger("AttackStep", i + 1);

            // 2. Wait for the animation to finish (using the time from your SO)
            // We use 'attackEndTime' to ensure he finishes the swing before starting the next
            float timer = 0f;
            while (timer < attack.attackEndTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        ResetCombo();
    }

    private void ResetCombo()
    {
        _isAttacking = false;
        _animator.SetBool("IsAttack", false);
        _animator.SetInteger("AttackStep", 0);
    }
}