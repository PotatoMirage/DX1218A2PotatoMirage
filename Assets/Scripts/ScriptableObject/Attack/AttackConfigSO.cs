using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfigSO", menuName = "Scriptable Objects/AttackConfigSO")]
public class AttackConfigSO : ScriptableObject
{
    [Header("Animation")]
    public string animationStateName; // e.g., "Attack1"
    public float transitionDuration = 0.1f;

    [Header("Timing")]
    [Tooltip("Normalized time (0-1) when the next attack can be queued")]
    public float comboUnlockTime = 0.5f;
    [Tooltip("Normalized time (0-1) when the animation is considered 'finished'")]
    public float attackEndTime = 0.9f;

    [Header("Impact")]
    public float impulseForce = 1f;
    public int damageAmount = 10;
}