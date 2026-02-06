using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<AttackConfigSO> comboChain;

    [Header("Dependencies")]
    // [FIX] Abstraction: Use InputReader SO instead of direct InputSystem class
    [SerializeField] private InputReader inputReader;
    // [FIX] Dependency: Reference Controller to check state
    [SerializeField] private PlayerController playerController;

    // State
    private List<IEnumerator> _attackQueue = new();
    private bool _isAttacking;
    private int _attackStep;
    private Animator _animator;

    public event System.Action<bool> OnAttackStateChanged;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // [FIX] Observer Pattern: Subscribe to SO event
        if (inputReader != null)
            inputReader.AttackEvent += HandleAttackInput;
    }

    private void OnDisable()
    {
        if (inputReader != null)
            inputReader.AttackEvent -= HandleAttackInput;
    }

    // [MODIFIED] Check State before attacking
    private void HandleAttackInput()
    {
        // 1. Guard Clause: If in Ranged Mode, do not perform Melee attacks
        if (playerController.IsRangedMode) return;

        if (_attackQueue.Count < comboChain.Count)
        {
            _attackQueue.Add(PerformAttackRoutine(_attackQueue.Count));

            if (_attackQueue.Count == 1 && !_isAttacking)
            {
                StartCombo();
            }
        }
    }

    // ... (Rest of your combo logic remains the same) ...

    private void StartCombo()
    {
        _isAttacking = true;
        _attackStep = 0;
        OnAttackStateChanged?.Invoke(true);
        _animator.SetBool("IsAttack", true);

        if (_attackQueue.Count > 0) StartCoroutine(_attackQueue[0]);
    }

    private IEnumerator PerformAttackRoutine(int stepIndex)
    {
        _attackStep = stepIndex + 1;
        _animator.SetInteger("AttackStep", _attackStep);

        AttackConfigSO currentAttack = comboChain[stepIndex];

        while (!IsAnimationReady(currentAttack.animationStateName, currentAttack.comboUnlockTime))
        {
            yield return null;
        }

        if (_attackStep < _attackQueue.Count)
        {
            StartCoroutine(_attackQueue[_attackStep]);
        }
        else
        {
            while (!IsAnimationReady(currentAttack.animationStateName, currentAttack.attackEndTime))
            {
                yield return null;
            }
            ResetCombo();
        }
    }

    private bool IsAnimationReady(string stateName, float threshold)
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(stateName) && info.normalizedTime >= threshold;
    }

    private void ResetCombo()
    {
        _isAttacking = false;
        _attackStep = 0;
        _attackQueue.Clear();
        _animator.SetBool("IsAttack", false);
        _animator.SetInteger("AttackStep", 0);
        OnAttackStateChanged?.Invoke(false);
    }
}