using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfigSO", menuName = "Scriptable Objects/AttackConfigSO")]
public class AttackConfigSO : ScriptableObject
{
    [Header("Animation")]
    public string AnimationName; // Matches the Parameter or Clip name in Animator
    public float TransitionDuration = 0.1f;

    [Header("Timing")]
    public float DamageWindowStart; // Time in seconds when hitbox activates
    public float DamageWindowEnd;   // Time in seconds when hitbox deactivates
    public float ComboResetTime;    // Time after which combo resets if no input

    [Header("Damage")]
    public int DamageAmount = 10;
    public float KnockbackForce = 5f;
}