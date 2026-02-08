using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private List<AttackConfigSO> comboChain;
    [SerializeField] private float damage = 10f;

    private Animator animator;
    private bool isAttacking;
    private int comboIndex;

    // Helper for AI
    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Called by EnemyController
    public void StartAttackCombo()
    {
        if (isAttacking) return;

        isAttacking = true;
        comboIndex = 0;
        animator.SetBool("IsAttack", true);

        PlayAttack(comboChain[comboIndex]);
    }

    private void PlayAttack(AttackConfigSO attackConfig)
    {
        animator.SetInteger("AttackStep", comboIndex + 1);
        animator.CrossFade(attackConfig.animationStateName, 0.1f);
    }

    public void EndAttack()
    {
        if (comboIndex < comboChain.Count - 1)
        {
            comboIndex++;
            PlayAttack(comboChain[comboIndex]);
        }
        else
        {
            ResetCombo();
        }
    }

    public void UnlockCombo() { }

    private void ResetCombo()
    {
        isAttacking = false;
        comboIndex = 0;
        animator.SetBool("IsAttack", false);
        animator.SetInteger("AttackStep", 0);
    }
}