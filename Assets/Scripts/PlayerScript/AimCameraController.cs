using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimCameraController : MonoBehaviour
{
    [SerializeField] private Transform yawTarget;
    [SerializeField] private Transform pitchTarget;

    [SerializeField] private PlayerController playerController;
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
        if (aimCam != null)
        {
            targetCameraSide = aimCam.CameraSide;
        }
    }

    void Start()
    {
        Vector3 angles = yawTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        if (lookInput != null && lookInput.asset != null)
            lookInput.asset.Enable();
    }

    private void OnEnable()
    {
        if (switchShouldInput != null && switchShouldInput.action != null)
        {
            switchShouldInput.action.Enable();
            switchShouldInput.action.performed += OnSwitchShoulder;
        }
    }

    private void OnDisable()
    {
        if (switchShouldInput != null && switchShouldInput.action != null)
        {
            switchShouldInput.action.performed -= OnSwitchShoulder;
        }
    }

    private void OnSwitchShoulder(InputAction.CallbackContext context)
    {
        if (playerController != null && !playerController.IsRangedMode) return;

        targetCameraSide = aimCam.CameraSide < 0.5f ? 1f : 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController != null && !playerController.IsRangedMode)
        {
            return;
        }

        Vector2 look = Vector2.zero;
        if (lookInput != null)
        {
            look = lookInput.action.ReadValue<Vector2>();
        }

        if (Mouse.current != null && Mouse.current.delta.IsActuated())
        {
            look *= mouseSensitivity;
        }

        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (aimCam != null)
        {
            aimCam.CameraSide = Mathf.Lerp(aimCam.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwitchSpeed);
        }
    }

    public void SetYawPitchFromCameraForward(Transform cameraTransform)
    {
        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude >= 0.001f)
        {
            yaw = Quaternion.LookRotation(flatForward).eulerAngles.y;
        }

        float currentPitch = cameraTransform.eulerAngles.x;
        if (currentPitch > 180) currentPitch -= 360;

        pitch = currentPitch;

        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}