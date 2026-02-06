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
    public event UnityAction<bool> AimEvent = delegate { }; // True = Start Aim, False = End
    public event UnityAction<bool> SprintEvent = delegate { }; 

    // Change: Use your specific class name
    private InputSystem_Actions _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new InputSystem_Actions();

            // Change: 'Player' is the name of the Action Map in your provided file
            _gameInput.Player.SetCallbacks(this);
        }
        _gameInput.Player.Enable();
    }

    private void OnDisable()
    {
        _gameInput.Player.Disable();
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

    // --- Required by Interface but unused for now ---
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
    public void OnSwitchShoulder(InputAction.CallbackContext context) { }
    public void OnZoom(InputAction.CallbackContext context) { }
}