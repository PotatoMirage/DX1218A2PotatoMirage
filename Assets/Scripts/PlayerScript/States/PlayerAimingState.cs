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
    }

    private void HandleRotation()
    {
        float yawCamera = Ctx.MainCamera.transform.rotation.eulerAngles.y;
        Ctx.transform.rotation = Quaternion.Slerp(Ctx.transform.rotation, Quaternion.Euler(0, yawCamera, 0), 20 * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 input = Ctx.CurrentMovementInput;

        // Move Logic
        Vector3 moveDirection = Ctx.transform.forward * input.y + Ctx.transform.right * input.x;
        Ctx.CharacterController.Move(moveDirection * Ctx.Stats.AimingWalkSpeed * Time.deltaTime);

        // --- SMOOTH ANIMATION ---
        // This function automatically interpolates the value over 'AnimationDampTime' seconds
        Ctx.Animator.SetFloat("InputX", input.x, Ctx.Stats.AnimationDampTime, Time.deltaTime);
        Ctx.Animator.SetFloat("InputY", input.y, Ctx.Stats.AnimationDampTime, Time.deltaTime);
    }

    private void SwitchState(PlayerBaseState newState)
    {
        ExitState();
        newState.EnterState();
        Ctx.CurrentState = newState;
    }
}