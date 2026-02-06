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
    public bool isAiming;
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _freeLookCamera;
    [SerializeField] private CinemachineCamera _aimCamera;
    [SerializeField] private Transform _mainCamera;

    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    // Data Inputs
    public Vector2 CurrentMovementInput { get; private set; }
    public bool IsAimingPressed { get; private set; }
    public bool IsSprintingPressed { get; private set; } // New Property

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
        _inputReader.SprintEvent += OnSprint; // Subscribe
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMove;
        _inputReader.AimEvent -= OnAim;
        _inputReader.SprintEvent -= OnSprint; // Unsubscribe
    }

    private void Update()
    {
        _currentState.UpdateState();
    }

    private void OnMove(Vector2 input) => CurrentMovementInput = input;
    private void OnAim(bool isAiming) => IsAimingPressed = isAiming;
    private void OnSprint(bool isSprinting) => IsSprintingPressed = isSprinting; // Listener
}