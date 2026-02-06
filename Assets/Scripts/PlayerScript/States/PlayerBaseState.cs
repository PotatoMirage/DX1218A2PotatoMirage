public abstract class PlayerBaseState
{
    protected PlayerController Ctx; // Context
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
}