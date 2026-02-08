public class PlayerStateFactory
{
    PlayerController context;

    public PlayerStateFactory(PlayerController currentContext)
    {
        context = currentContext;
    }

    public PlayerBaseState FreeLook() => new PlayerFreeLookState(context, this);
    public PlayerBaseState Aiming() => new PlayerAimingState(context, this);
    public PlayerBaseState Blocking() => new PlayerBlockState(context, this);
    public PlayerBaseState Jump() => new PlayerJumpState(context, this);
    public PlayerBaseState Crouch() => new PlayerCrouchState(context, this);
    public PlayerBaseState Roll() => new PlayerRollState(context, this);
}