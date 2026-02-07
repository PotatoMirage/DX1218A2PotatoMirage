using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _freeLookCamera;
    [SerializeField] private CinemachineCamera _aimCamera;
    [SerializeField] private Transform _mainCamera;

    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;
    public PlayerStats Stats => _stats;

    // Data Inputs
    public Vector2 CurrentMovementInput { get; private set; }
    public bool IsAimingPressed { get; private set; }
    public bool IsBlockingPressed { get; private set; }
    public bool IsSprintingPressed { get; private set; }
    public bool IsJumpPressed { get; private set; }     // [NEW]
    public bool IsCrouchPressed { get; private set; }   // [NEW]
    public bool IsRangedMode { get; private set; } = false;

    // Physics State
    public float VerticalVelocity; // [NEW] Handling Gravity
    public float RotationVelocity;

    public bool UseRootMotion { get; set; } = false;

    // Observer: Notify systems
    public event System.Action<bool> OnCombatModeChanged;

    // Getters
    public Animator Animator => _animator;
    public CharacterController CharacterController => _characterController;
    public CinemachineCamera FreeLookCamera => _freeLookCamera;
    public CinemachineCamera AimCamera => _aimCamera;
    public Transform MainCamera => _mainCamera;
    public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }

    private void Awake()
    {
        _states = new PlayerStateFactory(this);
        _currentState = _states.FreeLook();
        if (_mainCamera == null) _mainCamera = UnityEngine.Camera.main.transform;
    }

    private void Start() => _currentState.EnterState();
    private void Update() => _currentState.UpdateState();

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMove;
        _inputReader.AimEvent += OnAimOrBlock;
        _inputReader.SprintEvent += OnSprint;
        _inputReader.JumpEvent += OnJump;      // [NEW]
        _inputReader.CrouchEvent += OnCrouch;  // [NEW]
        _inputReader.SwitchCombatEvent += OnSwitchCombatMode;
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMove;
        _inputReader.AimEvent -= OnAimOrBlock;
        _inputReader.SprintEvent -= OnSprint;
        _inputReader.JumpEvent -= OnJump;      // [NEW]
        _inputReader.CrouchEvent -= OnCrouch;  // [NEW]
        _inputReader.SwitchCombatEvent -= OnSwitchCombatMode;
    }

    private void OnMove(Vector2 input) => CurrentMovementInput = input;
    private void OnSprint(bool isSprinting) => IsSprintingPressed = isSprinting;
    private void OnJump(bool isJumping) => IsJumpPressed = isJumping;     // [NEW]
    private void OnCrouch(bool isCrouching) => IsCrouchPressed = isCrouching; // [NEW]

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
            Vector3 velocity = _animator.deltaPosition;

            // 2. Apply Manual Gravity (since Root Motion usually ignores Y)
            // We multiply by deltaTime because VerticalVelocity is "per second"
            // but deltaPosition is "per frame".
            velocity.y += VerticalVelocity * Time.deltaTime;

            // 3. Move the Controller
            _characterController.Move(velocity);

            // 4. Apply Rotation from Animation
            transform.rotation *= _animator.deltaRotation;
        }
    }
}