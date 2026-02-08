using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public int MaxHealth = 100;

    public float WalkSpeed = 6f;
    public float RunSpeed = 8f;
    public float AimingWalkSpeed = 3f;
    public float BlockSpeed = 2f;
    public float CrouchSpeed = 3f;

    public float JumpHeight = 1.5f;
    public float Gravity = -9.81f;
    public float GravityMultiplier = 2f;

    public float RotationSmoothTime = 0.1f;
    public float AnimationDampTime = 0.1f;
}