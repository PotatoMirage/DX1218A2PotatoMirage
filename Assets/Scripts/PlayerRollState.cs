using UnityEngine;

public class PlayerRollState : PlayerBaseState
{
    private float _rollDuration = 0.8f; // Adjust to match your animation length
    private float _elapsedTime;

    public PlayerRollState(PlayerController context, PlayerStateFactory factory)
        : base(context, factory) { }

    public override void EnterState()
    {
        _elapsedTime = 0f;
        Ctx.FreeLookCamera.gameObject.SetActive(true);
        // Trigger Animation
        Ctx.Animator.SetBool("IsRolling", true);

        // Enable Root Motion so the animation drives the distance
        Ctx.UseRootMotion = true;

        // Optional: Disable collision with enemies while rolling?
        // Physics.IgnoreLayerCollision(playerLayer, enemyLayer, true);
    }

    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;

        CheckSwitchStates();

        // Apply Gravity (Important if rolling off a ledge)
        if (Ctx.CharacterController.isGrounded && Ctx.VerticalVelocity < 0)
        {
            Ctx.VerticalVelocity = -2f;
        }
        Ctx.VerticalVelocity += Ctx.Stats.Gravity * Time.deltaTime;
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("IsRolling", false);

        // Reset Collision or other flags if needed
    }

    public override void CheckSwitchStates()
    {
        // Exit roll when time is up
        if (_elapsedTime >= _rollDuration)
        {
            SwitchState(Factory.FreeLook());
        }
    }
}