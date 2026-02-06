using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WorldCrosshairController : MonoBehaviour
{
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private Transform rigAimTarget;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float crossHairOffsetMultiplier = 0.01f;
    [SerializeField] private LayerMask raycastMask = -0;
    [SerializeField] private Rig aimRig;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float aimTransitionSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = aimCamera.ScreenPointToRay(screenCenter);

        Vector3 targetPos;
        if(Physics.Raycast(ray, out RaycastHit hit, maxDistance, raycastMask))
        {
            targetPos = hit.point + hit.normal * crossHairOffsetMultiplier;
            crosshairUI.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            targetPos = ray.GetPoint(maxDistance);
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
        bool isAimingState = true; // Or: Input.GetMouseButton(1);

        // 2. Set the target
        float destination = isAimingState ? 1.0f : 0.0f;
        aimRig.weight = Mathf.Lerp(aimRig.weight, destination, Time.deltaTime * aimTransitionSpeed);
    }
}
