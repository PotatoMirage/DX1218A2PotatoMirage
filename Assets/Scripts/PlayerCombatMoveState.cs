// PlayerCombatMoveState.cs (Strafing)
using UnityEngine;

public class PlayerCombatMoveState : PlayerBaseState
{
    public PlayerCombatMoveState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState() { }
    public override void UpdateState()
    {
        // Strafe Logic: Player faces forward (camera dir or lock-on target), moves sideways
        Vector3 camForward = Ctx.MainCamera.forward;
        Vector3 camRight = Ctx.MainCamera.right;
        camForward.y = 0; camRight.y = 0;

        Vector3 moveDir = (camForward * Ctx.CurrentMovementInput.y + camRight * Ctx.CurrentMovementInput.x).normalized;
        Ctx.CharController.Move(Ctx.WalkSpeed * Time.deltaTime * moveDir);

        // Keep facing camera forward for strafing
        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        Ctx.transform.rotation = Quaternion.Slerp(Ctx.transform.rotation, targetRotation, Ctx.RotationSpeed * Time.deltaTime);

        // Update Blend Tree Params for Strafing
        Ctx.Animator.SetFloat(Ctx.AnimID_X, Ctx.CurrentMovementInput.x); // Strafe Left/Right
        Ctx.Animator.SetFloat(Ctx.AnimID_Y, Ctx.CurrentMovementInput.y); // Walk Fwd/Back
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchStates()
    {
        if (Ctx.CurrentMovementInput.magnitude < 0.1f) SwitchState(Factory.CombatIdle());
        if (!Ctx.IsCombatMode) SwitchState(Factory.Move());
        if (Ctx.IsBlocking) SwitchState(Factory.Block());
        // Check Attack Input
        // Check Dodge Input
    }
}