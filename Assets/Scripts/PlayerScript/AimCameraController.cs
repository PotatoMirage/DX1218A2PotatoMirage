using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimCameraController : MonoBehaviour
{
    [SerializeField] private Transform yawTarget;
    [SerializeField] private Transform pitchTarget;

    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private InputActionReference switchShouldInput;

    [SerializeField] private float mouseSensitivity = 0.05f;
    [SerializeField] private float sensitivity = 1.5f;

    [SerializeField] private float pitchMin = -40f;
    [SerializeField] private float pitchMax = 80f;

    [SerializeField] private CinemachineThirdPersonFollow aimCam;
    [SerializeField] private float shoulderSwitchSpeed = 5f;

    private float yaw;
    private float pitch;
    private float targetCameraSide;
    private void Awake()
    {
        aimCam = GetComponent<CinemachineThirdPersonFollow>();
        targetCameraSide = aimCam.CameraSide;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 angles = yawTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        lookInput.asset.Enable();
    }

    private void OnEnable()
    {
        switchShouldInput.action.Enable();
        switchShouldInput.action.performed += OnSwitchShoulder;
    }

    private void OnDisable()
    {
        switchShouldInput.action.Enable();
        switchShouldInput.action.performed += OnSwitchShoulder;
    }

    private void OnSwitchShoulder(InputAction.CallbackContext context)
    {
        targetCameraSide = aimCam.CameraSide < 0.5f ? 1f : 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 look = lookInput.action.ReadValue<Vector2>();

        if(Mouse.current != null && Mouse.current.delta.IsActuated())
        {
            look *= mouseSensitivity;
        }

        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        aimCam.CameraSide = Mathf.Lerp(aimCam.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwitchSpeed);
    }

    public void SetYawPitchFromCameraForward(Transform cameraTransform)
    {
        // 1. Handle Yaw (Left/Right)
        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude >= 0.001f)
        {
            yaw = Quaternion.LookRotation(flatForward).eulerAngles.y;
        }

        // FIX #2: Handle Pitch (Up/Down)
        // Get the x angle (pitch) from the camera
        float currentPitch = cameraTransform.eulerAngles.x;

        // Unity angles are 0-360. We need to convert values like 350 to -10 to match our clamp range.
        if (currentPitch > 180) currentPitch -= 360;

        pitch = currentPitch;

        // Apply immediately so there is no single-frame jitter
        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
