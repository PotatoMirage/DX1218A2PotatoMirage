using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private List<AttackConfigSO> lightComboChain;
    [SerializeField] private List<AttackConfigSO> heavyComboChain;

    [SerializeField] private InputReader inputReader;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AttackHandler attackHandler;

    // State Variables
    private List<AttackConfigSO> currentChain;
    private int comboIndex;
    private bool isAttacking;
    private bool comboUnlocked;
    private bool inputBuffered;

    private Animator animator;

    public event System.Action<bool> OnAttackStateChanged;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.AttackEvent += HandleLightAttack;
            inputReader.HeavyAttackEvent += HandleHeavyAttack;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.AttackEvent -= HandleLightAttack;
            inputReader.HeavyAttackEvent -= HandleHeavyAttack;
        }
    }

    private void HandleLightAttack() => HandleInput(lightComboChain);
    private void HandleHeavyAttack() => HandleInput(heavyComboChain);

    private void HandleInput(List<AttackConfigSO> targetChain)
    {
        if (playerController.IsRangedMode) return;

        if (!isAttacking)
        {
            currentChain = targetChain;
            StartCombo();
        }
        else if (comboUnlocked)
        {
            inputBuffered = true;
            currentChain = targetChain;
        }
    }

    private void StartCombo()
    {
        isAttacking = true;
        comboIndex = 0;

        playerController.UseRootMotion = true;
        OnAttackStateChanged?.Invoke(true);
        animator.SetBool("IsAttack", true);

        PlayAttack(currentChain[comboIndex]);
    }

    private void PlayAttack(AttackConfigSO attackConfig)
    {
        comboUnlocked = false;
        inputBuffered = false;

        if (attackHandler != null)
        {
            attackHandler.SetupAttack(attackConfig);
        }

        int typeValue = (currentChain == heavyComboChain) ? 1 : 0;
        animator.SetInteger("AttackType", typeValue);
        animator.SetInteger("AttackStep", comboIndex + 1);
    }

    public void UnlockCombo()
    {
        comboUnlocked = true;
    }

    public void EndAttack()
    {
        if (inputBuffered && comboIndex < currentChain.Count - 1)
        {
            comboIndex++;
            PlayAttack(currentChain[comboIndex]);
        }
        else
        {
            FinishCombo();
        }
    }

    private void FinishCombo()
    {
        isAttacking = false;
        comboIndex = 0;
        comboUnlocked = false;
        inputBuffered = false;
        animator.SetBool("IsAttack", false);
        animator.SetInteger("AttackStep", 0);
        animator.SetInteger("AttackType", 0);

        OnAttackStateChanged?.Invoke(false);
    }
}