using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    public PlayerCrouchState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool("IsCrouching", true); // Ensure you have this parameter in Animator
        Ctx.FreeLookCamera.gameObject.SetActive(true);
        // Optional: Reduce CharacterController Height here
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleMovement();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("IsCrouching", false);
        Ctx.FreeLookCamera.gameObject.SetActive(false);
        // Optional: Reset CharacterController Height here
    }

    public override void CheckSwitchStates()
    {
        // If crouch button released
        if (!Ctx.IsCrouchPressed)
        {
            SwitchState(Factory.FreeLook());
        }
        // If Jump pressed?
        if (Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
    }

    private void HandleMovement()
    {
        Vector2 input = Ctx.CurrentMovementInput;

        // Gravity
        if (Ctx.CharacterController.isGrounded && Ctx.VerticalVelocity < 0) Ctx.VerticalVelocity = -2f;
        Ctx.VerticalVelocity += Ctx.Stats.Gravity * Time.deltaTime;

        Vector3 movement = new Vector3(input.x, 0, input.y);

        if (movement.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + Ctx.MainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(Ctx.transform.eulerAngles.y, targetAngle, ref Ctx.RotationVelocity, Ctx.Stats.RotationSmoothTime);
            Ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Use CrouchSpeed
            Vector3 finalMove = (moveDir.normalized * Ctx.Stats.CrouchSpeed) + (Vector3.up * Ctx.VerticalVelocity);
            Ctx.CharacterController.Move(finalMove * Time.deltaTime);

            Ctx.Animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime); // Force walk anim
        }
        else
        {
            Ctx.CharacterController.Move(Vector3.up * Ctx.VerticalVelocity * Time.deltaTime);
            Ctx.Animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
        }
    }
}