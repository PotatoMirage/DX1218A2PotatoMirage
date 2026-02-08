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
        Ctx.Animator.SetBool("IsJumping", false);

        Ctx.FreeLookCamera.gameObject.SetActive(true);

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
        if (Ctx.IsRollPressed && Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Roll());
            return;
        }
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
        Vector3 movement = new(input.x, 0, input.y);

        if (movement.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + Ctx.MainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(Ctx.transform.eulerAngles.y, targetAngle, ref Ctx.RotationVelocity, Ctx.Stats.RotationSmoothTime);
            Ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        float targetSpeed = 0f;
        if (movement.magnitude > 0)
        {
            targetSpeed = Ctx.IsSprintingPressed ? 1f : 0.5f;
        }

        Ctx.Animator.SetFloat("Speed", targetSpeed, Ctx.Stats.AnimationDampTime, Time.deltaTime);
    }
}