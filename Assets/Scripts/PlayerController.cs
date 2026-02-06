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
    public bool IsAimingPressed { get; private set; }   // Ranged RMB
    public bool IsBlockingPressed { get; private set; } // Melee RMB
    public bool IsSprintingPressed { get; private set; }
    public bool IsRangedMode { get; private set; } = false;

    // Observer: Notify systems (Animation, UI)
    public event System.Action<bool> OnCombatModeChanged;

    public float RotationVelocity;

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
        _inputReader.AimEvent += OnAimOrBlock; // Renamed handler
        _inputReader.SprintEvent += OnSprint;
        _inputReader.SwitchCombatEvent += OnSwitchCombatMode;
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMove;
        _inputReader.AimEvent -= OnAimOrBlock;
        _inputReader.SprintEvent -= OnSprint;
        _inputReader.SwitchCombatEvent -= OnSwitchCombatMode;
    }

    private void OnMove(Vector2 input) => CurrentMovementInput = input;
    private void OnSprint(bool isSprinting) => IsSprintingPressed = isSprinting;

    // [MODIFIED] Route Input based on State
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

        // Reset Inputs on switch to prevent stuck states
        IsAimingPressed = false;
        IsBlockingPressed = false;

        OnCombatModeChanged?.Invoke(IsRangedMode);
    }
}