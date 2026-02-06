// PlayerAttackState.cs
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private float _timePassed;
    private float _clipLength;
    private float _clipSpeed = 1;

    public PlayerAttackState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        Ctx.IsAttacking = true;
        _timePassed = 0f;

        // Get Current Attack Data
        int comboIdx = Ctx.CurrentComboIndex;
        if (comboIdx >= Ctx.AttackCombo.Length) comboIdx = 0;

        AttackConfigSO attack = Ctx.AttackCombo[comboIdx];

        // Play Animation
        Ctx.Animator.CrossFade(attack.AnimationName, attack.TransitionDuration);

        // Normally you get clip length from Animator, simplistic approach here:
        _clipLength = 1.0f; // Replace with actual retrieval logic or store in ConfigSO
    }

    public override void UpdateState()
    {
        _timePassed += Time.deltaTime;

        // Rotation towards input/camera during attack start (optional)
        // Hitbox activation logic here (Observer pattern usually handles this)
    }

    public override void FixedUpdateState() { }
    public override void ExitState()
    {
        Ctx.IsAttacking = false;
        Ctx.LastAttackTime = Time.time;
        // Advance combo index
        Ctx.CurrentComboIndex++;
        if (Ctx.CurrentComboIndex >= Ctx.AttackCombo.Length) Ctx.CurrentComboIndex = 0;
    }

    public override void CheckSwitchStates()
    {
        if (_timePassed >= _clipLength)
        {
            SwitchState(Factory.CombatIdle());
        }
    }
}