using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float _minJumpDuration = 0.5f;
    private float _elapsedTime;

    public PlayerJumpState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        _elapsedTime = 0f;
        Ctx.FreeLookCamera.gameObject.SetActive(true);
        Ctx.Animator.SetBool("IsJumping", true);
        Ctx.UseRootMotion = true;

        Ctx.PlayJumpSound();
    }

    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("IsJumping", false);
    }

    public override void CheckSwitchStates()
    {
        if (_elapsedTime < _minJumpDuration) return;

        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.FreeLook());
        }
    }

    private void HandleGravity()
    {
        Ctx.VerticalVelocity += Ctx.Stats.Gravity * Time.deltaTime;
    }
}