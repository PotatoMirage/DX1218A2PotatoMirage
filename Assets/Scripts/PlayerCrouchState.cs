using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    public PlayerCrouchState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool("IsCrouching", true);
        Ctx.FreeLookCamera.gameObject.SetActive(true);

        // [FIX] Enable Root Motion for Crouching
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
        Ctx.Animator.SetBool("IsCrouching", false);
        Ctx.FreeLookCamera.gameObject.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsCrouchPressed)
        {
            SwitchState(Factory.FreeLook());
        }
        else if (Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
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

        // 1. Rotation (Script-driven for better control)
        if (movement.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + Ctx.MainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(Ctx.transform.eulerAngles.y, targetAngle, ref Ctx.RotationVelocity, Ctx.Stats.RotationSmoothTime);
            Ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // 2. Animator Input (Drives Root Motion)
        // Set Speed parameter to a low value (e.g., 0.5 or lower) to trigger Crouch Walk
        // You might need a specific "CrouchSpeed" parameter in your Animator if "Speed" controls standing walk.
        // Assuming "Speed" is shared:
        float targetSpeed = movement.magnitude > 0 ? 0.5f : 0f;

        Ctx.Animator.SetFloat("Speed", targetSpeed, 0.1f, Time.deltaTime);
    }
}