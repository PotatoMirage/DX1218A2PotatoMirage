using UnityEngine;

// Responsible strictly for updating Animator parameters based on events
public class PlayerAnimationObserver : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _animator;

    private static readonly int IsRangedHash = Animator.StringToHash("IsRangedMode");
    private static readonly int IsBlockingHash = Animator.StringToHash("IsBlocking");

    private void OnEnable()
    {
        _playerController.OnCombatModeChanged += UpdateCombatModeAnimation;
    }

    private void OnDisable()
    {
        _playerController.OnCombatModeChanged -= UpdateCombatModeAnimation;
    }

    private void UpdateCombatModeAnimation(bool isRanged)
    {
        _animator.SetBool(IsRangedHash, isRanged);
    }

    // Can be called by State Machine behaviors or events
    public void SetBlocking(bool isBlocking)
    {
        _animator.SetBool(IsBlockingHash, isBlocking);
    }
}