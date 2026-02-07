using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    // Timer to ignore 'isGrounded' at the very start of the jump
    // otherwise the code sees we are still on the ground before the animation lifts us.
    private float _minJumpDuration = 0.5f;
    private float _elapsedTime;

    public PlayerJumpState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        _elapsedTime = 0f;
        Ctx.FreeLookCamera.gameObject.SetActive(true);
        Ctx.Animator.SetBool("IsJumping", true);
        Ctx.UseRootMotion = true;
    }

    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;

        // Gravity is handled by PlayerController.OnAnimatorMove, 
        // but we need to update the velocity variable for it to use.
        HandleGravity();

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("IsJumping", false);
    }

    public override void CheckSwitchStates()
    {
        // 1. Wait for minimum time (prevents instant exit)
        if (_elapsedTime < _minJumpDuration) return;

        // 2. Check Animation Status
        AnimatorStateInfo info = Ctx.Animator.GetCurrentAnimatorStateInfo(0);

        // If we are Grounded AND the animation is either done or not the Jump tag anymore
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.FreeLook());
        }
    }

    private void HandleGravity()
    {
        // If using Root Motion for Y-axis (rare), do nothing.
        // If Root Motion is only X/Z, we need to apply gravity manually:
        Ctx.VerticalVelocity += Ctx.Stats.Gravity * Time.deltaTime;
    }
}