// PlayerMoveState.cs (Standard 8-way movement)
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerController ctx, PlayerStateFactory factory) : base(ctx, factory) { }

    public override void EnterState() { }
    public override void UpdateState()
    {
        HandleMovement();
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchStates()
    {
        if (Ctx.CurrentMovementInput.magnitude < 0.1f) SwitchState(Factory.Idle());
        if (Ctx.IsCombatMode) SwitchState(Factory.CombatMove());
    }

    void HandleMovement()
    {
        Vector3 camForward = Ctx.MainCamera.forward;
        Vector3 camRight = Ctx.MainCamera.right;
        camForward.y = 0; camRight.y = 0;

        Vector3 moveDir = (camForward * Ctx.CurrentMovementInput.y + camRight * Ctx.CurrentMovementInput.x).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            Ctx.CharController.Move(Ctx.RunSpeed * Time.deltaTime * moveDir);
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            Ctx.transform.rotation = Quaternion.Slerp(Ctx.transform.rotation, targetRotation, Ctx.RotationSpeed * Time.deltaTime);
            Ctx.Animator.SetFloat(Ctx.AnimID_Speed, 1); // 1 = Run
        }
    }
}