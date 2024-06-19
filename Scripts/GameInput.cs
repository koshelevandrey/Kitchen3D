using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        // Subscribe to interact button press
        playerInputActions.Player.Interact.performed += InteractOnPerformed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternateOnPerformed;
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj) {
        if (OnInteractAction != null) {
            // Publish event
            OnInteractAction(this, EventArgs.Empty);
        }

        // Same as above but shorter:
        // OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternateOnPerformed(InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}