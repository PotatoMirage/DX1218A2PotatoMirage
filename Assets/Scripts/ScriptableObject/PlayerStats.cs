using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement Speeds")]
    public float WalkSpeed = 6f;
    public float RunSpeed = 8f;
    public float AimingWalkSpeed = 3f;
    public float BlockSpeed = 2f;    // [NEW]
    public float CrouchSpeed = 3f;   // [NEW]

    [Header("Physics")]
    public float JumpHeight = 1.5f;  // [NEW]
    public float Gravity = -9.81f;   // [NEW]
    public float GravityMultiplier = 2f; // Helps jump feel snappier

    [Header("Smoothing")]
    public float RotationSmoothTime = 0.1f;
    public float AnimationDampTime = 0.1f;
}