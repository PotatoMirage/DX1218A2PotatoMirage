public abstract class PlayerBaseState
{
    protected PlayerController Ctx;
    protected PlayerStateFactory Factory;

    public PlayerBaseState(PlayerController context, PlayerStateFactory factory)
    {
        Ctx = context;
        Factory = factory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();

    protected void SwitchState(PlayerBaseState newState)
    {
        ExitState();

        newState.EnterState();

        Ctx.CurrentState = newState;
    }
}