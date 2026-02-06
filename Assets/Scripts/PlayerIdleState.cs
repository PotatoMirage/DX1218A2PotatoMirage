// PlayerIdleState.cs
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetFloat(Ctx.AnimID_Speed, 0);
        Ctx.Animator.SetFloat(Ctx.AnimID_X, 0);
        Ctx.Animator.SetFloat(Ctx.AnimID_Y, 0);
    }
    public override void UpdateState() { }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchStates()
    {
        if (Ctx.CurrentMovementInput.magnitude > 0.1f) SwitchState(Factory.Move());
        if (Ctx.IsCombatMode) SwitchState(Factory.CombatIdle());
        if (Ctx.IsDead) SwitchState(Factory.Death());
    }
}