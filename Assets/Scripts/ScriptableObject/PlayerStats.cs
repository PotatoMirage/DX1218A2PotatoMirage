using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float WalkSpeed = 6f;
    public float RunSpeed = 8f;
    public float AimingWalkSpeed = 3f;
    public float RotationSmoothTime = 0.1f;
    public float AnimationDampTime = 0.1f;

    [Header("Combat")]
    public float DodgeSpeed = 12f;
    public float DodgeDuration = 0.5f;
    public float DodgeCooldown = 1f;
    public float StunDuration = 2f;
    public float MaxHealth = 100f;
    public float MaxBlockStamina = 50f; // "Poise" for blocking
}