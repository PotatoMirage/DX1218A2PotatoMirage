using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera freeLookCamera;
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private Transform mainCamera;

    private PlayerBaseState currentState;
    private PlayerStateFactory states;
    public PlayerStats Stats => stats;

    // Data Inputs
    public Vector2 CurrentMovementInput { get; private set; }
    public bool IsAimingPressed { get; private set; }
    public bool IsBlockingPressed { get; private set; }
    public bool IsSprintingPressed { get; private set; }
    public bool IsJumpPressed { get; private set; }     // [NEW]
    public bool IsCrouchPressed { get; private set; }   // [NEW]
    public bool IsRangedMode { get; private set; } = false;
    public bool IsLockedOn { get; set; } = false;
    public bool IsRollPressed { get; private set; }
    // Physics State
    public float VerticalVelocity; // [NEW] Handling Gravity
    public float RotationVelocity;

    public bool UseRootMotion { get; set; } = false;

    // Observer: Notify systems
    public event System.Action<bool> OnCombatModeChanged;

    // Getters
    public Animator Animator => animator;
    public CharacterController CharacterController => characterController;
    public CinemachineCamera FreeLookCamera => freeLookCamera;
    public CinemachineCamera AimCamera => aimCamera;
    public Transform MainCamera => mainCamera;
    public PlayerBaseState CurrentState { get => currentState; set => currentState = value; }
    public Transform LockOnTarget { get; private set; }
    public void SetLockOnState(bool state)
    {
        IsLockedOn = state;
    }

    public void SetLockOnTarget(Transform target)
    {
        LockOnTarget = target;
    }
    private void Awake()
    {
        states = new PlayerStateFactory(this);
        currentState = states.FreeLook();
        if (mainCamera == null) mainCamera = UnityEngine.Camera.main.transform;
    }

    private void Start() => currentState.EnterState();
    private void Update() => currentState.UpdateState();

    private void OnEnable()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.AimEvent += OnAimOrBlock;
        inputReader.SprintEvent += OnSprint;
        inputReader.JumpEvent += OnJump;      // [NEW]
        inputReader.CrouchEvent += OnCrouch;  // [NEW]
        inputReader.SwitchCombatEvent += OnSwitchCombatMode;
        inputReader.RollEvent += OnRoll;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.AimEvent -= OnAimOrBlock;
        inputReader.SprintEvent -= OnSprint;
        inputReader.JumpEvent -= OnJump;      // [NEW]
        inputReader.CrouchEvent -= OnCrouch;  // [NEW]
        inputReader.SwitchCombatEvent -= OnSwitchCombatMode;
        inputReader.RollEvent -= OnRoll;
    }

    private void OnMove(Vector2 input) => CurrentMovementInput = input;
    private void OnSprint(bool isSprinting) => IsSprintingPressed = isSprinting;
    private void OnJump(bool isJumping) => IsJumpPressed = isJumping;     // [NEW]
    private void OnCrouch(bool isCrouching) => IsCrouchPressed = isCrouching; // [NEW]
    private void OnRoll(bool isRolling) => IsRollPressed = isRolling;
    private void OnAimOrBlock(bool isPressed)
    {
        if (IsRangedMode)
        {
            IsAimingPressed = isPressed;
            IsBlockingPressed = false;
        }
        else
        {
            IsBlockingPressed = isPressed;
            IsAimingPressed = false;
        }
    }

    private void OnSwitchCombatMode()
    {
        IsRangedMode = !IsRangedMode;
        IsAimingPressed = false;
        IsBlockingPressed = false;
        OnCombatModeChanged?.Invoke(IsRangedMode);
    }
    private void OnAnimatorMove()
    {
        if (UseRootMotion)
        {
            // 1. Get movement from Animation
            Vector3 velocity = animator.deltaPosition;

            // 2. Apply Manual Gravity (since Root Motion usually ignores Y)
            // We multiply by deltaTime because VerticalVelocity is "per second"
            // but deltaPosition is "per frame".
            velocity.y += VerticalVelocity * Time.deltaTime;

            // 3. Move the Controller
            characterController.Move(velocity);

            // 4. Apply Rotation from Animation
            transform.rotation *= animator.deltaRotation;
        }
    }
}