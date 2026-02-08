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
    private int _comboIndex;

    // Helper for AI
    public bool IsAttacking => _isAttacking;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Called by EnemyController
    public void StartAttackCombo()
    {
        if (_isAttacking) return;

        _isAttacking = true;
        _comboIndex = 0;
        _animator.SetBool("IsAttack", true);

        PlayAttack(comboChain[_comboIndex]);
    }

    private void PlayAttack(AttackConfigSO attackConfig)
    {
        _animator.SetInteger("AttackStep", _comboIndex + 1);
        _animator.CrossFade(attackConfig.animationStateName, 0.1f);
    }

    // --- ANIMATION EVENTS ---

    // REQUIRED: Add "EndAttack" event to the end of ALL Enemy Attack Animations
    public void EndAttack()
    {
        // Check if there is another attack in the chain
        if (_comboIndex < comboChain.Count - 1)
        {
            _comboIndex++;
            PlayAttack(comboChain[_comboIndex]);
        }
        else
        {
            ResetCombo();
        }
    }

    // Optional: Only needed if you want the enemy to rotate mid-attack
    public void UnlockCombo() { }

    private void ResetCombo()
    {
        _isAttacking = false;
        _comboIndex = 0;
        _animator.SetBool("IsAttack", false);
        _animator.SetInteger("AttackStep", 0);
    }
}