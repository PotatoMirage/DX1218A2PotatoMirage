using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        // [FIX] Turn the FreeLook camera back on, because FreeLookState disabled it on Exit
        Ctx.FreeLookCamera.gameObject.SetActive(true);

        Ctx.Animator.SetBool("IsBlocking", true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        // Ensure the player is rooted (or add HandleMovement if you want strafing)
        Ctx.CharacterController.Move(Vector3.zero);
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("IsBlocking", false);

        // [FIX] Disable the camera when leaving (so AimState can take over if needed)
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
    }
}