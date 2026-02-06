// Replace your entire PlayerBaseState.cs with this
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

    // [FIX] Add this method to resolve the error in PlayerBlockState
    protected void SwitchState(PlayerBaseState newState)
    {
        // 1. Exit current state
        ExitState();

        // 2. Enter new state
        newState.EnterState();

        // 3. Update context tracking
        Ctx.CurrentState = newState;
    }
}