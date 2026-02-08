using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private HealthComponent healthComponent;

    [SerializeField] private CinemachineCamera freeLookCamera;
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private Transform mainCamera;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip blockHitSound;
    [SerializeField] private GameObject blockSparksPrefab;

    [SerializeField] private Transform blockEffectPos;
    [SerializeField] private AudioClip jumpSound;

    private PlayerBaseState currentState;
    private PlayerStateFactory states;
    public PlayerStats Stats => stats;

    public Vector2 CurrentMovementInput { get; private set; }
    public bool IsAimingPressed { get; private set; }
    public bool IsBlockingPressed { get; private set; }
    public bool IsSprintingPressed { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsCrouchPressed { get; private set; }
    public bool IsRangedMode { get; private set; } = false;
    public bool IsLockedOn { get; set; } = false;
    public bool IsRollPressed { get; private set; }

    public float VerticalVelocity;
    public float RotationVelocity;
    public bool UseRootMotion { get; set; } = false;

    public event System.Action<bool> OnCombatModeChanged;

    public Animator Animator => animator;
    public CharacterController CharacterController => characterController;
    public CinemachineCamera FreeLookCamera => freeLookCamera;
    public CinemachineCamera AimCamera => aimCamera;
    public Transform MainCamera => mainCamera;
    public PlayerBaseState CurrentState { get => currentState; set => currentState = value; }
    public Transform LockOnTarget { get; private set; }

    public void SetLockOnState(bool state) { IsLockedOn = state; }
    public void SetLockOnTarget(Transform target) { LockOnTarget = target; }

    private void Awake()
    {
        states = new PlayerStateFactory(this);
        currentState = states.FreeLook();
        if (mainCamera == null) mainCamera = UnityEngine.Camera.main.transform;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (blockEffectPos == null) blockEffectPos = transform;
    }

    private void Start() => currentState.EnterState();
    private void Update() => currentState.UpdateState();

    private void OnEnable()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.AimEvent += OnAimOrBlock;
        inputReader.SprintEvent += OnSprint;
        inputReader.JumpEvent += OnJump;
        inputReader.CrouchEvent += OnCrouch;
        inputReader.SwitchCombatEvent += OnSwitchCombatMode;
        inputReader.RollEvent += OnRoll;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.AimEvent -= OnAimOrBlock;
        inputReader.SprintEvent -= OnSprint;
        inputReader.JumpEvent -= OnJump;
        inputReader.CrouchEvent -= OnCrouch;
        inputReader.SwitchCombatEvent -= OnSwitchCombatMode;
        inputReader.RollEvent -= OnRoll;
    }

    private void OnMove(Vector2 input) => CurrentMovementInput = input;
    private void OnSprint(bool isSprinting) => IsSprintingPressed = isSprinting;
    private void OnJump(bool isJumping) => IsJumpPressed = isJumping;
    private void OnCrouch(bool isCrouching) => IsCrouchPressed = isCrouching;
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
            Vector3 velocity = animator.deltaPosition;
            velocity.y += VerticalVelocity * Time.deltaTime;
            characterController.Move(velocity);
            transform.rotation *= animator.deltaRotation;
        }
    }

    public void PlayFootstep()
    {
        if (footstepSounds.Length > 0 && audioSource != null)
        {
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }
    public void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(jumpSound);
        }
    }
    public void OnTakeHit(int damageAmount)
    {
        if (currentState is PlayerBlockState)
        {
            if (blockHitSound && audioSource) audioSource.PlayOneShot(blockHitSound);
            if (blockSparksPrefab) Instantiate(blockSparksPrefab, blockEffectPos.position, Quaternion.LookRotation(transform.forward));
        }
        else
        {
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(damageAmount);
            }
        }
    }
}