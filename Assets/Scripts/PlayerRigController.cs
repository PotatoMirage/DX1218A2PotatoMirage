using UnityEngine;
using UnityEngine.Animations.Rigging; // Requires Animation Rigging Package

public class PlayerRigController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rig aimRig; // Assign your "Ranged Aim Rig" here

    [Header("Settings")]
    [SerializeField] private float blendSpeed = 5f;

    private float _targetWeight;

    private void Awake()
    {
        // Default: If game starts in Melee mode, Rig is off (0).
        _targetWeight = 0f;
        aimRig.weight = 0f;
    }

    private void OnEnable()
    {
        if (playerController != null)
            playerController.OnCombatModeChanged += HandleCombatModeChanged;
    }

    private void OnDisable()
    {
        if (playerController != null)
            playerController.OnCombatModeChanged -= HandleCombatModeChanged;
    }

    private void HandleCombatModeChanged(bool isRanged)
    {
        // If Ranged (True) -> Weight 1. If Melee (False) -> Weight 0.
        _targetWeight = isRanged ? 1f : 0f;
    }

    private void Update()
    {
        // Smoothly blend the rig weight
        // This prevents the character from "snapping" instantly between aiming and not aiming
        if (Mathf.Abs(aimRig.weight - _targetWeight) > 0.001f)
        {
            aimRig.weight = Mathf.Lerp(aimRig.weight, _targetWeight, Time.deltaTime * blendSpeed);
        }
    }
}