using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    private PlayerInputActions playerInput;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    public void Awake() {
        playerInput = new PlayerInputActions();
        playerInput.Enable();

        playerInput.Player.Interact.performed += Interact_performed;
        playerInput.Player.InteractAlternate.performed += InteractAlternate_performed;
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);

    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
