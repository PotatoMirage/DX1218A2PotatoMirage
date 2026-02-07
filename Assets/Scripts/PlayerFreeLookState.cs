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
        // 1. Capture Inputs
        Vector2 input = Ctx.CurrentMovementInput;
        Vector3 movement = new Vector3(input.x, 0, input.y);

        // 2. Calculate the "Target" Animation Speed based on your rules
        // Default to 0 (Idle)
        float targetAnimSpeed = 0f;

        if (movement.magnitude > 0)
        {
            // If moving, check if sprinting (1.0) or walking (0.5)
            targetAnimSpeed = Ctx.IsSprintingPressed ? 1f : 0.5f;
        }

        // 3. Apply to Animator
        // We use "0.1f" as dampTime to make the blend smooth, so it doesn't snap instantly
        Ctx.Animator.SetFloat("Speed", targetAnimSpeed, 0.1f, Time.deltaTime);

        // --- Physics Movement Logic (Existing) ---
        if (movement.magnitude == 0) return;

        float moveSpeed = Ctx.IsSprintingPressed ? Ctx.Stats.RunSpeed : Ctx.Stats.WalkSpeed;

        float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg +
                            Ctx.MainCamera.transform.eulerAngles.y;

        float angle = Mathf.SmoothDampAngle(Ctx.transform.eulerAngles.y, targetAngle,
                                            ref Ctx.RotationVelocity, Ctx.Stats.RotationSmoothTime);

        Ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        Ctx.CharacterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
    }

    private void SwitchState(PlayerBaseState newState)
    {
        ExitState();
        newState.EnterState();
        Ctx.CurrentState = newState;
    }
}