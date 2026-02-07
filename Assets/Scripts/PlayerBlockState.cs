using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        Ctx.FreeLookCamera.gameObject.SetActive(true);
        Ctx.Animator.SetBool("IsBlocking", true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleRotation();
        HandleMovement();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("IsBlocking", false);
        Ctx.FreeLookCamera.gameObject.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsBlockingPressed)
        {
            SwitchState(Factory.FreeLook());
        }
        else if (Ctx.IsRangedMode)
        {
            SwitchState(Factory.FreeLook());
        }
        // Optional: Allow Jumping from Block?
        // if (Ctx.IsJumpPressed) SwitchState(Factory.Jump());
    }

    private void HandleRotation()
    {
        // While blocking, we usually face the camera direction to block incoming attacks
        float yawCamera = Ctx.MainCamera.transform.rotation.eulerAngles.y;
        Ctx.transform.rotation = Quaternion.Slerp(Ctx.transform.rotation, Quaternion.Euler(0, yawCamera, 0), 20 * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector2 input = Ctx.CurrentMovementInput;

        // Apply Gravity
        if (Ctx.CharacterController.isGrounded && Ctx.VerticalVelocity < 0)
        {
            Ctx.VerticalVelocity = -2f;
        }
        Ctx.VerticalVelocity += Ctx.Stats.Gravity * Time.deltaTime;

        // Strafe Logic
        Vector3 moveDirection = Ctx.transform.forward * input.y + Ctx.transform.right * input.x;
        moveDirection.y = 0; // Keep horizontal only

        // Combine Move + Gravity
        Vector3 finalMove = (moveDirection * Ctx.Stats.BlockSpeed) + (Vector3.up * Ctx.VerticalVelocity);

        Ctx.CharacterController.Move(finalMove * Time.deltaTime);

        // Animate
        Ctx.Animator.SetFloat("InputX", input.x, Ctx.Stats.AnimationDampTime, Time.deltaTime);
        Ctx.Animator.SetFloat("InputY", input.y, Ctx.Stats.AnimationDampTime, Time.deltaTime);
    }
}