using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    public InputReader Input;
    public Animator Animator;
    public CharacterController CharController;
    public Transform MainCamera;

    [Header("Combat Config")]
    public AttackConfigSO[] AttackCombo; // Drag your 5 attack SOs here
    public float WalkSpeed = 2f;
    public float RunSpeed = 5f;
    public float RotationSpeed = 15f;

    // State Machine
    public PlayerBaseState CurrentState;
    public PlayerStateFactory StateFactory;

    // Runtime Data
    [HideInInspector] public Vector2 CurrentMovementInput;
    [HideInInspector] public bool IsCombatMode;
    [HideInInspector] public bool IsBlocking;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsDead;
    [HideInInspector] public bool IsStunned;
    [HideInInspector] public int CurrentComboIndex = 0;
    [HideInInspector] public float LastAttackTime = 0;

    // Hashing Animator Parameters for performance
    public readonly int AnimID_Speed = Animator.StringToHash("Speed");
    public readonly int AnimID_X = Animator.StringToHash("InputX");
    public readonly int AnimID_Y = Animator.StringToHash("InputY");
    public readonly int AnimID_CombatMode = Animator.StringToHash("CombatMode");
    public readonly int AnimID_Block = Animator.StringToHash("Blocking");
    public readonly int AnimID_Dodge = Animator.StringToHash("TriggerDodge");
    public readonly int AnimID_Stun = Animator.StringToHash("TriggerStun");
    public readonly int AnimID_Death = Animator.StringToHash("TriggerDeath");
    // Attacks can be triggered via CrossFade, so we might not need parameters for them

    private void Awake()
    {
        StateFactory = new PlayerStateFactory(this);
        CurrentState = StateFactory.Idle();
        CurrentState.EnterState();
    }

    private void OnEnable()
    {
        Input.MoveEvent += OnMove;
        Input.ToggleCombatEvent += OnToggleCombat;
        Input.BlockEventStart += () => IsBlocking = true;
        Input.BlockEventCancel += () => IsBlocking = false;
        // Attack and Dodge are handled inside states via CheckSwitchStates or direct polling
    }

    private void OnDisable()
    {
        Input.MoveEvent -= OnMove;
        Input.ToggleCombatEvent -= OnToggleCombat;
        // Unsubscribe others...
    }

    private void Update()
    {
        CurrentState.UpdateState();
        CurrentState.CheckSwitchStates();
    }

    private void FixedUpdate()
    {
        CurrentState.FixedUpdateState();
    }

    private void OnMove(Vector2 input) => CurrentMovementInput = input;

    private void OnToggleCombat()
    {
        IsCombatMode = !IsCombatMode;
        Animator.SetBool(AnimID_CombatMode, IsCombatMode);
    }

    // Public method for external components (like enemies) to call
    public void TakeDamage(int amount)
    {
        if (IsBlocking)
        {
            // Calculate block break logic here
            // If block broken: IsStunned = true;
            Debug.Log("Blocked!");
            return;
        }

        // Apply Damage
        Animator.Play("TakeDamage", 0, 0f); // Or Trigger
        // If Health <= 0: IsDead = true;
    }
}