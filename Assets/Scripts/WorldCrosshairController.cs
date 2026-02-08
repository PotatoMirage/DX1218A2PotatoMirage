using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WorldCrosshairController : MonoBehaviour
{
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private Transform rigAimTarget;

    [SerializeField] private Camera aimCamera;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float crossHairOffsetMultiplier = 0.01f;
    [SerializeField] private LayerMask raycastMask = -1;
    [SerializeField] private float aimTransitionSpeed = 10f;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rig aimRig;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (playerController == null && !playerController.IsAimingPressed) return;

        bool isRanged = playerController.IsRangedMode;

        if (crosshairUI != null && crosshairUI.gameObject.activeSelf != isRanged)
        {
            crosshairUI.gameObject.SetActive(isRanged);
        }

        if (aimRig != null)
        {
            float targetWeight = isRanged ? 1.0f : 0.0f;
            aimRig.weight = Mathf.Lerp(aimRig.weight, targetWeight, Time.deltaTime * aimTransitionSpeed);
        }

        if (isRanged)
        {
            Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0);
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