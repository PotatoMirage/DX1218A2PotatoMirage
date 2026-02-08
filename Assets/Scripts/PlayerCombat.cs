using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combo Chains")]
    [SerializeField] private List<AttackConfigSO> lightComboChain;
    [SerializeField] private List<AttackConfigSO> heavyComboChain;

    [Header("Dependencies")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private PlayerController playerController;

    // State Variables
    private List<AttackConfigSO> _currentChain;
    private int _comboIndex;
    private bool _isAttacking;
    private bool _comboUnlocked;
    private bool _inputBuffered;

    private Animator _animator;
    private Coroutine _failsafeRoutine;

    public event System.Action<bool> OnAttackStateChanged;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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

    // --- Input Handling ---

    private void HandleLightAttack() => HandleInput(lightComboChain);
    private void HandleHeavyAttack() => HandleInput(heavyComboChain);

    private void HandleInput(List<AttackConfigSO> targetChain)
    {
        if (playerController.IsRangedMode) return;

        if (!_isAttacking)
        {
            _currentChain = targetChain;
            StartCombo();
        }
        else if (_comboUnlocked)
        {
            _inputBuffered = true;
            _currentChain = targetChain;
        }
    }

    // --- Combat Logic ---

    private void StartCombo()
    {
        _isAttacking = true;
        _comboIndex = 0;

        playerController.UseRootMotion = true;
        OnAttackStateChanged?.Invoke(true);
        _animator.SetBool("IsAttack", true);

        PlayAttack(_currentChain[_comboIndex]);
    }

    private void PlayAttack(AttackConfigSO attackConfig)
    {
        _comboUnlocked = false;
        _inputBuffered = false;

        // [METHOD A] - Set Parameters to guide the Animator

        // 1. Determine Type: 0 = Light, 1 = Heavy
        int typeValue = (_currentChain == heavyComboChain) ? 1 : 0;
        _animator.SetInteger("AttackType", typeValue);

        // 2. Set Step: Triggers the transition in the Animator
        _animator.SetInteger("AttackStep", _comboIndex + 1);
    }

    // --- ANIMATION EVENTS ---

    public void UnlockCombo()
    {
        _comboUnlocked = true;
    }

    public void EndAttack()
    {
        if (_failsafeRoutine != null) StopCoroutine(_failsafeRoutine);

        if (_inputBuffered && _comboIndex < _currentChain.Count - 1)
        {
            _comboIndex++;
            PlayAttack(_currentChain[_comboIndex]);
        }
        else
        {
            FinishCombo();
        }
    }

    private void FinishCombo()
    {
        if (_failsafeRoutine != null) StopCoroutine(_failsafeRoutine);

        _isAttacking = false;
        _comboIndex = 0;
        _comboUnlocked = false;
        _inputBuffered = false;

        playerController.UseRootMotion = false;
        _animator.SetBool("IsAttack", false);
        _animator.SetInteger("AttackStep", 0);
        // Reset Type to default (optional, but cleaner)
        _animator.SetInteger("AttackType", 0);

        OnAttackStateChanged?.Invoke(false);
    }
}