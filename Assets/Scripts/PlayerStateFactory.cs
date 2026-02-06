public class PlayerStateFactory
{
    private PlayerController _context;

    // Cache states to avoid GC allocations
    private PlayerBaseState _idle, _move, _combatIdle, _combatMove, _attack, _block, _dodge, _stun, _death;

    public PlayerStateFactory(PlayerController currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle() => _idle ??= new PlayerIdleState(_context, this);
    public PlayerBaseState Move() => _move ??= new PlayerMoveState(_context, this);
    public PlayerBaseState CombatIdle() => _combatIdle ??= new PlayerCombatIdleState(_context, this);
    public PlayerBaseState CombatMove() => _combatMove ??= new PlayerCombatMoveState(_context, this);
    public PlayerBaseState Attack() => _attack ??= new PlayerAttackState(_context, this);
    public PlayerBaseState Block() => _block ??= new PlayerBlockState(_context, this);
    public PlayerBaseState Dodge() => _dodge ??= new PlayerDodgeState(_context, this);
    public PlayerBaseState Stun() => _stun ??= new PlayerStunState(_context, this);
    public PlayerBaseState Death() => _death ??= new PlayerDeathState(_context, this);
}