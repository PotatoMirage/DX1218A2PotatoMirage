using UnityEngine;

public class PlayerRollState : PlayerBaseState
{
    private float rollDuration = 0.8f;
    private float elapsedTime;

    public PlayerRollState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        elapsedTime = 0f;
        Ctx.FreeLookCamera.gameObject.SetActive(true);
        Ctx.Animator.SetBool("IsRolling", true);

        Ctx.UseRootMotion = true;
    }

    public override void UpdateState()
    {
        elapsedTime += Time.deltaTime;

        CheckSwitchStates();
        if (Ctx.CharacterController.isGrounded && Ctx.VerticalVelocity < 0)
        {
            Ctx.VerticalVelocity = -2f;
        }
        Ctx.VerticalVelocity += Ctx.Stats.Gravity * Time.deltaTime;
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("IsRolling", false);

    }

    public override void CheckSwitchStates()
    {
        if (elapsedTime >= rollDuration)
        {
            SwitchState(Factory.FreeLook());
        }
    }
}