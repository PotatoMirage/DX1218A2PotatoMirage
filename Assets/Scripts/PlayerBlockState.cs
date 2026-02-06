// PlayerBlockState.cs
public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.AnimID_Block, true);
    }
    public override void UpdateState()
    {
        // Block Loop Animation handles itself via bool
    }
    public override void FixedUpdateState() { }
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.AnimID_Block, false);
    }
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsBlocking) SwitchState(Factory.CombatIdle());
        if (Ctx.IsStunned) SwitchState(Factory.Stun());
    }
}