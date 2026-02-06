using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<AttackConfigSO> comboChain; // Drag your SOs here

    [Header("Dependencies")]
    [SerializeField] private InputSystem_Actions inputActions; // Or reference your InputReader

    // State
    private List<IEnumerator> _attackQueue = new();
    private bool _isAttacking;
    private int _attackStep;
    private Animator _animator;

    // Observer Event for other scripts (e.g., Movement needs to know if we are attacking)
    public event System.Action<bool> OnAttackStateChanged;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // Setup Input (Observer Pattern)
        // Ideally, this comes from a central InputReader SO, but direct initializing for now:
        inputActions = new InputSystem_Actions();
        inputActions.Player.Attack.performed += HandleAttackInput;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void HandleAttackInput(InputAction.CallbackContext context)
    {
        if (_attackQueue.Count < comboChain.Count)
        {
            // Add the specific attack logic to the queue
            _attackQueue.Add(PerformAttackRoutine(_attackQueue.Count));

            if (_attackQueue.Count == 1 && !_isAttacking)
            {
                StartCombo();
            }
        }
    }

    private void StartCombo()
    {
        _isAttacking = true;
        _attackStep = 0;
        OnAttackStateChanged?.Invoke(true); // Notify listeners
        _animator.SetBool("IsAttack", true);

        if (_attackQueue.Count > 0) StartCoroutine(_attackQueue[0]);
    }

    private IEnumerator PerformAttackRoutine(int stepIndex)
    {
        _attackStep = stepIndex + 1;
        _animator.SetInteger("AttackStep", _attackStep);

        AttackConfigSO currentAttack = comboChain[stepIndex];

        // Wait for animation to reach the "Unlock" point
        while (!IsAnimationReady(currentAttack.animationStateName, currentAttack.comboUnlockTime))
        {
            yield return null;
        }

        // Check if there is a next attack in the queue
        if (_attackStep < _attackQueue.Count)
        {
            StartCoroutine(_attackQueue[_attackStep]);
        }
        else
        {
            // Wait for full finish if no more attacks
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