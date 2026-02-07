using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    public PlayerFreeLookState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool("IsAiming", false);
        Ctx.Animator.SetBool("IsBlocking", false);
        Ctx.Animator.SetBool("IsCrouching", false);
        Ctx.Animator.SetBool("IsJumping", false); // Safety reset

        Ctx.FreeLookCamera.gameObject.SetActive(true);

        // [IMPORTANT] Enable Root Motion for walking/running
        Ctx.UseRootMotion = true;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleGravity();
        HandleMovement();
    }

    public override void ExitState()
    {
        Ctx.FreeLookCamera.gameObject.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsRangedMode && Ctx.IsAimingPressed) SwitchState(Factory.Aiming());
        else if (!Ctx.IsRangedMode && Ctx.IsBlockingPressed) SwitchState(Factory.Blocking());
        else if (Ctx.IsJumpPressed && Ctx.CharacterController.isGrounded) SwitchState(Factory.Jump());
        else if (Ctx.IsCrouchPressed) SwitchState(Factory.Crouch());
    }

    private void HandleGravity()
    {
        if (Ctx.CharacterController.isGrounded && Ctx.VerticalVelocity < 0)
        {
            Ctx.VerticalVelocity = -2f;
        }
        Ctx.VerticalVelocity += Ctx.Stats.Gravity * Time.deltaTime;
    }

    private void HandleMovement()
    {
        Vector2 input = Ctx.CurrentMovementInput;
        Vector3 movement = new Vector3(input.x, 0, input.y);

        // 1. Handle Rotation (Manual)
        // Root Motion usually handles position well, but turning is often cleaner when script-driven
        // unless you have high-quality "Turn in Place" animations.
        if (movement.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + Ctx.MainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(Ctx.transform.eulerAngles.y, targetAngle, ref Ctx.RotationVelocity, Ctx.Stats.RotationSmoothTime);
            Ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // 2. Set Animator Parameters (This drives the Root Motion)
        // Instead of moving the CharacterController, we tell the Animator: "Go Fast"
        float targetSpeed = 0f;
        if (movement.magnitude > 0)
        {
            targetSpeed = Ctx.IsSprintingPressed ? 1f : 0.5f;
        }

        // We use DampTime to smooth out the transitions
        Ctx.Animator.SetFloat("Speed", targetSpeed, Ctx.Stats.AnimationDampTime, Time.deltaTime);
    }
}