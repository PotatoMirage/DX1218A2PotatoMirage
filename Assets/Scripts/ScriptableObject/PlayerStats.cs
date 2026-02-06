using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float WalkSpeed = 6f;
    public float RunSpeed = 8f;
    public float AimingWalkSpeed = 3f;
    public float RotationSmoothTime = 0.1f;

    public float AnimationDampTime = 0.1f;
}