using UnityEngine;

public class PlayerAimingState : PlayerBaseState
{
    public PlayerAimingState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool("IsAiming", true);
        Ctx.AimCamera.gameObject.SetActive(true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleRotation();
        HandleMovement();
    }

    public override void ExitState()
    {
        Ctx.AimCamera.gameObject.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsAimingPressed)
        {
            SwitchState(Factory.FreeLook());
        }
        // Optional: Allow jumping while aiming?
        // if (Ctx.IsJumpPressed && Ctx.CharacterController.isGrounded) SwitchState(Factory.Jump());
    }

    private void HandleRotation()
    {
        float yawCamera = Ctx.MainCamera.transform.rotation.eulerAngles.y;
        Ctx.transform.rotation = Quaternion.Slerp(Ctx.transform.rotation, Quaternion.Euler(0, yawCamera, 0), 20 * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 input = Ctx.CurrentMovementInput;

        // --- Gravity ---
        if (Ctx.CharacterController.isGrounded && Ctx.VerticalVelocity < 0)
        {
            Ctx.VerticalVelocity = -2f;
        }
        Ctx.VerticalVelocity += Ctx.Stats.Gravity * Time.deltaTime;

        // --- Movement ---
        Vector3 moveDirection = Ctx.transform.forward * input.y + Ctx.transform.right * input.x;

        // Combine Strafing + Gravity
        Vector3 finalMove = (moveDirection * Ctx.Stats.AimingWalkSpeed) + (Vector3.up * Ctx.VerticalVelocity);

        Ctx.CharacterController.Move(finalMove * Time.deltaTime);

        // Animation
        Ctx.Animator.SetFloat("InputX", input.x, Ctx.Stats.AnimationDampTime, Time.deltaTime);
        Ctx.Animator.SetFloat("InputY", input.y, Ctx.Stats.AnimationDampTime, Time.deltaTime);
    }
}