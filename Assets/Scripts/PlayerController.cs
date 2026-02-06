using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    [SerializeField] private WorldCrosshairController crosshairController;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _freeLookCamera;
    [SerializeField] private CinemachineCamera _aimCamera;
    [SerializeField] private Transform _mainCamera;

    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    // Data Inputs
    public Vector2 CurrentMovementInput { get; private set; }
    public bool IsAimingPressed { get; private set; }
    public bool IsSprintingPressed { get; private set; }

    // [NEW] Combat State
    public bool IsRangedMode { get; private set; } = false; // Default to Melee

    // Observer Subject: Notify other systems (UI, Weapons) when mode changes
    public event System.Action<bool> OnCombatModeChanged;

    public float RotationVelocity;

    // Getters
    public Animator Animator => _animator;
    public CharacterController CharacterController => _characterController;
    public PlayerStats Stats => _stats;
    public CinemachineCamera FreeLookCamera => _freeLookCamera;
    public CinemachineCamera AimCamera => _aimCamera;
    public Transform MainCamera => _mainCamera;
    public PlayerBaseState CurrentState { set => _currentState = value; }

    private void Awake()
    {
        _states = new PlayerStateFactory(this);
        _currentState = _states.FreeLook();
        if (_mainCamera == null) _mainCamera = UnityEngine.Camera.main.transform;
    }

    private void Start()
    {
        _currentState.EnterState();
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMove;
        _inputReader.AimEvent += OnAim;
        _inputReader.SprintEvent += OnSprint;

        // [NEW] Subscribe to switch event
        _inputReader.SwitchCombatEvent += OnSwitchCombatMode;
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMove;
        _inputReader.AimEvent -= OnAim;
        _inputReader.SprintEvent -= OnSprint;
        _inputReader.SwitchCombatEvent -= OnSwitchCombatMode;
    }

    private void Update()
    {
        _currentState.UpdateState();
    }

    private void OnMove(Vector2 input) => CurrentMovementInput = input;
    private void OnSprint(bool isSprinting) => IsSprintingPressed = isSprinting;

    // [MODIFIED] Logic to gatekeep Aiming
    private void OnAim(bool isAiming)
    {
        // Only allow aiming if we are in Ranged Mode
        if (IsRangedMode)
        {
            IsAimingPressed = isAiming;
        }
        else
        {
            IsAimingPressed = false;
        }
    }

    // [NEW] Handle Mode Switching
    private void OnSwitchCombatMode()
    {
        IsRangedMode = !IsRangedMode;

        // Notify observers (like UI or Animation layers)
        OnCombatModeChanged?.Invoke(IsRangedMode);

        // Visual Feedback
        Debug.Log($"Combat Mode Switched. Ranged: {IsRangedMode}");

        // If we switch to Melee while holding Right Click, force exit Aim state
        if (!IsRangedMode)
        {
            IsAimingPressed = false;
            // The PlayerAimingState will exit automatically in its Update cycle 
            // when it detects IsAimingPressed is false.
        }
    }
}