using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    public PlayerFreeLookState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool("IsAiming", false);
        Ctx.FreeLookCamera.gameObject.SetActive(true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleMovement();
    }

    public override void ExitState()
    {
        Ctx.FreeLookCamera.gameObject.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        // Existing Aim Logic
        if (Ctx.IsRangedMode && Ctx.IsAimingPressed)
        {
            SwitchState(Factory.Aiming());
        }
        // New Block Logic
        else if (!Ctx.IsRangedMode && Ctx.IsBlockingPressed)
        {
            SwitchState(Factory.Blocking());
        }
    }

    private void HandleMovement()
    {
        Vector2 input = Ctx.CurrentMovementInput;
        Vector3 movement = new Vector3(input.x, 0, input.y);

        if (movement.magnitude == 0)
        {
            // Smoothly stop the animation
            Ctx.Animator.SetBool("IsWalking", false);
            Ctx.Animator.SetBool("IsRunning", false);
            return;
        }

        // Determine Speed and Animation Status
        float speed = Ctx.IsSprintingPressed ? Ctx.Stats.RunSpeed : Ctx.Stats.WalkSpeed;
        bool isRunning = Ctx.IsSprintingPressed;

        // Set Animator
        Ctx.Animator.SetBool("IsWalking", !isRunning); // True if moving but not sprinting
        Ctx.Animator.SetBool("IsRunning", isRunning);  // True if sprinting

        // Calculate Rotation
        float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg +
                            Ctx.MainCamera.transform.eulerAngles.y;

        float angle = Mathf.SmoothDampAngle(Ctx.transform.eulerAngles.y, targetAngle,
                                            ref Ctx.RotationVelocity, Ctx.Stats.RotationSmoothTime);

        Ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // Move Forward relative to the new rotation
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        Ctx.CharacterController.Move(moveDir.normalized * speed * Time.deltaTime);
    }

    private void SwitchState(PlayerBaseState newState)
    {
        ExitState();
        newState.EnterState();
        Ctx.CurrentState = newState;
    }
}