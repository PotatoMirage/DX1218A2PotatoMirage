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

    [Header("Audio & VFX")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Assign multiple clips for variation")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip blockHitSound;
    [SerializeField] private GameObject blockSparksPrefab;
    [Tooltip("Where the block effect appears (e.g. Shield or Chest)")]
    [SerializeField] private Transform blockEffectPos;

    private PlayerBaseState currentState;
    private PlayerStateFactory states;
    public PlayerStats Stats => stats;

    // Data Inputs
    public Vector2 CurrentMovementInput { get; private set; }
    public bool IsAimingPressed { get; private set; }
    public bool IsBlockingPressed { get; private set; }
    public bool IsSprintingPressed { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsCrouchPressed { get; private set; }
    public bool IsRangedMode { get; private set; } = false;
    public bool IsLockedOn { get; set; } = false;
    public bool IsRollPressed { get; private set; }

    // Physics State
    public float VerticalVelocity;
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

    public void SetLockOnState(bool state) { IsLockedOn = state; }
    public void SetLockOnTarget(Transform target) { LockOnTarget = target; }

    private void Awake()
    {
        states = new PlayerStateFactory(this);
        currentState = states.FreeLook();
        if (mainCamera == null) mainCamera = UnityEngine.Camera.main.transform;

        // Auto-get AudioSource if missing
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (blockEffectPos == null) blockEffectPos = transform; // Default to player root
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

    // ---------------- NEW METHODS ----------------

    // 1. Called by Animation Events
    public void PlayFootstep()
    {
        if (footstepSounds.Length > 0 && audioSource != null)
        {
            // Pick a random footstep
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            // Randomize pitch slightly for realism
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }

    // 2. Called by Enemy AttackHandler
    public void OnTakeHit(int damageAmount)
    {
        // Check if we are currently in the Block State
        if (currentState is PlayerBlockState)
        {
            // --- BLOCKED! ---
            if (blockHitSound && audioSource)
                audioSource.PlayOneShot(blockHitSound);

            if (blockSparksPrefab)
                Instantiate(blockSparksPrefab, blockEffectPos.position, Quaternion.LookRotation(transform.forward));

            // Optional: You could reduce damage here instead of ignoring it
            // stats.TakeDamage(damageAmount * 0.1f); 
        }
        else
        {
            // --- NOT BLOCKED (Take Damage) ---
            // stats.TakeDamage(damageAmount);

            // If you have a hit reaction animation, trigger it here
            // animator.SetTrigger("HitReaction");
        }
    }
}