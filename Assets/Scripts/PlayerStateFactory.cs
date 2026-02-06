public class PlayerStateFactory
{
    PlayerController _context;

    public PlayerStateFactory(PlayerController currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState FreeLook() => new PlayerFreeLookState(_context, this);
    public PlayerBaseState Aiming() => new PlayerAimingState(_context, this);
}