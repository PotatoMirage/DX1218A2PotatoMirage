using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfigSO", menuName = "Scriptable Objects/AttackConfigSO")]
public class AttackConfigSO : ScriptableObject
{
    [Header("Animation")]
    public string animationStateName; // e.g., "Attack1"
    public float transitionDuration = 0.1f;

    [Header("Impact")]
    public float impulseForce = 1f;
    public int damageAmount = 10;
}