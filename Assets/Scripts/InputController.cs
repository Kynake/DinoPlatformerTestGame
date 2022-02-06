using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    private static InputController _instance = null;

    private PlayerControls _playerControls;

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void OnEnable()
    {
        _playerControls = new PlayerControls();

        SubscribeToAction(_playerControls.PlayerMovement.Move, OnMoveAction);
        SubscribeToAction(_playerControls.PlayerMovement.Jump, OnJumpAction);

        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();

        UnsubscribeFromAction(_playerControls.PlayerMovement.Move, OnMoveAction);
        UnsubscribeFromAction(_playerControls.PlayerMovement.Jump, OnJumpAction);
    }

    private void OnDestroy()
    {
        if(_instance != this)
        {
            return;
        }

        _instance = null;
    }

    private void SubscribeToAction(InputAction action, Action<InputAction.CallbackContext> actionEventHandler)
    {
        action.started += actionEventHandler;
        action.performed += actionEventHandler;
        action.canceled += actionEventHandler;
    }

    private void UnsubscribeFromAction(InputAction action, Action<InputAction.CallbackContext> actionEventHandler)
    {
        action.started -= actionEventHandler;
        action.performed -= actionEventHandler;
        action.canceled -= actionEventHandler;
    }

    /** A quick overview of InputAction callback states:
     *
     * Disabled:  The action is disabled and will not detect any input.
     * Waiting:   The action is enabled and waiting for input. This is the state the button is in when not pressed.
     * Started:   The action was started. This is the state the button immediately goes, and stays on, while it's being pressed.
     * Performed: The action was performed. This triggers exactly once, right after Started callback when the button is pressed. The action does not stay in this state.
     * Canceled:  The action was canceled or stopped. This triggers exactly once when the button is released, then the action immediately goes to the Waiting state.
     */

    // Player Controls
    public delegate void Move(Vector2 direction);
    public static event Move OnMove;
    private void OnMoveAction(InputAction.CallbackContext context)
    {
        switch(context.phase)
        {
            case InputActionPhase.Performed:
                OnMove?.Invoke(context.ReadValue<Vector2>());
                break;

            case InputActionPhase.Canceled:
                OnMove?.Invoke(Vector2.zero);
                break;
        }
    }

    public delegate void Jump();
    public static event Jump OnJump;
    private void OnJumpAction(InputAction.CallbackContext context)
    {
        if(context.performed) {
            OnJump?.Invoke();
        }
    }
}
