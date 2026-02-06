using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        // Stop movement while blocking
        Ctx.Animator.SetBool("IsBlocking", true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        // Zero out movement logic here if you want them rooted
        Ctx.CharacterController.Move(Vector3.zero);
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("IsBlocking", false);
    }

    public override void CheckSwitchStates()
    {
        // 1. If we let go of Block (RMB), return to FreeLook
        if (!Ctx.IsBlockingPressed)
        {
            SwitchState(Factory.FreeLook());
        }
        // 2. If we switch to Ranged mode abruptly, exit block
        else if (Ctx.IsRangedMode)
        {
            SwitchState(Factory.FreeLook());
        }
    }
}