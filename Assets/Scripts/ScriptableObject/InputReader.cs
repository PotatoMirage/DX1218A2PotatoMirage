using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, InputSystem_Actions.IPlayerActions
{
    // Observer Pattern: Events for other scripts to listen to
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction<Vector2> LookEvent = delegate { };
    public event UnityAction AttackEvent = delegate { };
    public event UnityAction HeavyAttackEvent = delegate { };
    public event UnityAction<bool> AimEvent = delegate { };
    public event UnityAction<bool> SprintEvent = delegate { };
    public event UnityAction<bool> JumpEvent = delegate { };   // [NEW]
    public event UnityAction<bool> CrouchEvent = delegate { }; // [NEW]

    public event UnityAction SwitchCombatEvent = delegate { };

    private InputSystem_Actions _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new InputSystem_Actions();
            _gameInput.Player.SetCallbacks(this);
        }
        _gameInput.Player.Enable();
    }

    private void OnDisable()
    {
        _gameInput?.Player.Disable();
    }

    // --- Interface Implementation ---

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            AttackEvent.Invoke();
    }
    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            HeavyAttackEvent.Invoke();
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            AimEvent.Invoke(true);
        else if (context.phase == InputActionPhase.Canceled)
            AimEvent.Invoke(false);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            SprintEvent.Invoke(true);
        else if (context.phase == InputActionPhase.Canceled)
            SprintEvent.Invoke(false);
    }

    // [NEW] Jump Implementation
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            JumpEvent.Invoke(true);
        else if (context.phase == InputActionPhase.Canceled)
            JumpEvent.Invoke(false);
    }

    // [NEW] Crouch Implementation
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            CrouchEvent.Invoke(true);
        else if (context.phase == InputActionPhase.Canceled)
            CrouchEvent.Invoke(false);
    }

    public void OnSwitchCombat(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            SwitchCombatEvent.Invoke();
    }

    // Unused
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
    public void OnSwitchShoulder(InputAction.CallbackContext context) { }
    public void OnZoom(InputAction.CallbackContext context) { }
}