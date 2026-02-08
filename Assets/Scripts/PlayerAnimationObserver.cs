using UnityEngine;

public class PlayerAnimationObserver : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;

    private static readonly int IsRangedHash = Animator.StringToHash("IsRangedMode");
    private static readonly int IsBlockingHash = Animator.StringToHash("IsBlocking");

    private void OnEnable()
    {
        playerController.OnCombatModeChanged += UpdateCombatModeAnimation;
    }

    private void OnDisable()
    {
        playerController.OnCombatModeChanged -= UpdateCombatModeAnimation;
    }

    private void UpdateCombatModeAnimation(bool isRanged)
    {
        animator.SetBool(IsRangedHash, isRanged);
    }

    public void SetBlocking(bool isBlocking)
    {
        animator.SetBool(IsBlockingHash, isBlocking);
    }
}