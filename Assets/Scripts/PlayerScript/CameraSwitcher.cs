using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    [SerializeField] private CinemachineCamera freelookCam;
    [SerializeField] private CinemachineCamera aimCam;
    [SerializeField] private CinemachineCamera lockOnCam;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private RectTransform lockOnReticle;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float lockOnRadius = 20f;
    [SerializeField] private float unlockDistance = 30f;

    private bool isAiming = false;
    private bool isLockedOn = false;
    private Transform currentLockTarget;
    private AimCameraController aimCamController;
    private CinemachineInputAxisController freeLookInput;

    public event Action<bool> OnAimStateChanged;

    private void Awake()
    {
        aimCamController = aimCam.GetComponent<AimCameraController>();
        freeLookInput = freelookCam.GetComponent<CinemachineInputAxisController>();
    }

    private void Start()
    {
        if (lockOnReticle != null) lockOnReticle.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        inputReader.AimEvent += OnAim;
        inputReader.LockOnEvent += OnLockOn;
    }

    private void OnDisable()
    {
        inputReader.AimEvent -= OnAim;
        inputReader.LockOnEvent -= OnLockOn;
    }

    private void LateUpdate()
    {
        if (isLockedOn)
        {
            HandleLockOnMonitor();
            UpdateLockOnReticlePosition();
        }
    }

    private void OnAim(bool isPressed)
    {
        if (isPressed && !isAiming && player.IsRangedMode)
        {
            if (isLockedOn) DisengageLockOn();
            EnterAimMode();
        }
        else if ((!isPressed || !player.IsRangedMode) && isAiming)
        {
            ExitAimMode();
        }
    }

    private void OnLockOn()
    {
        if (isAiming) return;

        if (isLockedOn)
        {
            DisengageLockOn();
        }
        else
        {
            player.SetLockOnState(true);
            isLockedOn = true;
            FindAndLockTarget();
        }
    }

    private void FindAndLockTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(player.transform.position, lockOnRadius, enemyLayer);
        Transform bestTarget = null;
        float closestDistToCenter = float.MaxValue;
        Vector2 screenCenter = new(Screen.width / 2f, Screen.height / 2f);

        foreach (Collider enemy in enemies)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(enemy.transform.position);
            if (screenPos.z < 0) continue;

            float dist = Vector2.Distance(screenCenter, screenPos);
            if (dist < closestDistToCenter)
            {
                closestDistToCenter = dist;
                bestTarget = enemy.transform;
            }
        }

        if (bestTarget != null)
        {
            EngageLockOn(bestTarget);
        }
    }

    private void EngageLockOn(Transform target)
    {
        currentLockTarget = target;
        player.SetLockOnTarget(currentLockTarget);

        lockOnCam.Target.LookAtTarget = currentLockTarget;
        lockOnCam.Target.TrackingTarget = player.transform;
        lockOnCam.Priority = 15;

        if (lockOnReticle != null) lockOnReticle.gameObject.SetActive(true);
        if (freeLookInput != null) freeLookInput.enabled = false;
    }

    private void DisengageLockOn()
    {
        isLockedOn = false;
        currentLockTarget = null;

        player.SetLockOnState(false);
        player.SetLockOnTarget(null);

        lockOnCam.Priority = 0;
        lockOnCam.Target.LookAtTarget = null;

        if (lockOnReticle != null) lockOnReticle.gameObject.SetActive(false);
        if (freeLookInput != null) freeLookInput.enabled = true;
    }

    private void HandleLockOnMonitor()
    {
        if (currentLockTarget == null || !currentLockTarget.gameObject.activeInHierarchy)
        {
            DisengageLockOn();
            return;
        }

        float distance = Vector3.Distance(player.transform.position, currentLockTarget.position);
        if (distance > unlockDistance)
        {
            DisengageLockOn();
        }
    }

    private void UpdateLockOnReticlePosition()
    {
        if (lockOnReticle != null && currentLockTarget != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(currentLockTarget.position);

            if (screenPos.z > 0)
            {
                lockOnReticle.position = screenPos;
                lockOnReticle.gameObject.SetActive(true);
            }
            else
            {
                lockOnReticle.gameObject.SetActive(false);
            }
        }
    }

    private void EnterAimMode()
    {
        isAiming = true;
        OnAimStateChanged?.Invoke(true);
        SnapAimCameraToPlayerForward();

        aimCam.Priority = 20;
        freelookCam.Priority = 10;

        if (crosshairUI != null) crosshairUI.SetActive(true);
        if (freeLookInput != null) freeLookInput.enabled = false;
    }

    private void ExitAimMode()
    {
        isAiming = false;
        OnAimStateChanged?.Invoke(false);
        SnapFreeLookBehindPlayer();

        aimCam.Priority = 10;
        freelookCam.Priority = 20;

        if (crosshairUI != null) crosshairUI.SetActive(false);
        if (freeLookInput != null) freeLookInput.enabled = true;
    }

    private void SnapFreeLookBehindPlayer()
    {
        CinemachineOrbitalFollow orbitalFollow = freelookCam.GetComponent<CinemachineOrbitalFollow>();
        if (orbitalFollow != null)
        {
            Vector3 forward = aimCam.transform.forward;
            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            orbitalFollow.HorizontalAxis.Value = angle;
        }
    }

    private void SnapAimCameraToPlayerForward()
    {
        aimCamController.SetYawPitchFromCameraForward(freelookCam.transform);
    }
}