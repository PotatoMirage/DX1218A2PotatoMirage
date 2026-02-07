using UnityEngine;
using UnityEngine.Animations.Rigging; // Re-added for Rig control

public class WorldCrosshairController : MonoBehaviour
{
    [Header("UI & Targets")]
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private Transform rigAimTarget;

    [Header("Configuration")]
    [SerializeField] private Camera aimCamera;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float crossHairOffsetMultiplier = 0.01f;
    [SerializeField] private LayerMask raycastMask = -1;
    [SerializeField] private float aimTransitionSpeed = 10f; // Re-added

    [Header("Dependencies")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rig aimRig; // Re-added

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (playerController == null && !playerController.IsAimingPressed) return;

        // Determine State
        bool isRanged = playerController.IsRangedMode;

        // 1. Handle Crosshair Visibility
        // Only toggle if the state is different to avoid constant overhead
        if (crosshairUI != null && crosshairUI.gameObject.activeSelf != isRanged)
        {
            crosshairUI.gameObject.SetActive(isRanged);
        }

        // 2. Handle Rig Weight (Smooth Toggle)
        if (aimRig != null)
        {
            float targetWeight = isRanged ? 1.0f : 0.0f;
            aimRig.weight = Mathf.Lerp(aimRig.weight, targetWeight, Time.deltaTime * aimTransitionSpeed);
        }

        // 3. Handle Position & Raycast (Only calculate if Ranged)
        if (isRanged)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Ray ray = aimCamera.ScreenPointToRay(screenCenter);

            Vector3 targetPos;
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, raycastMask))
            {
                targetPos = hit.point + hit.normal * crossHairOffsetMultiplier;
                if (crosshairUI != null)
                    crosshairUI.rotation = Quaternion.LookRotation(hit.normal);
            }
            else
            {
                targetPos = ray.GetPoint(maxDistance);
                if (crosshairUI != null)
                    crosshairUI.forward = aimCamera.transform.forward;
            }

            if (crosshairUI != null)
            {
                crosshairUI.position = targetPos;
            }
            if (rigAimTarget != null)
            {
                rigAimTarget.position = targetPos;
            }
        }
    }
}