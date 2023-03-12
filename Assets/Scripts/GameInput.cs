using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {

    public event Action OnInteractAction; 
    private PlayerInputActions _playerInputActions;

    private void Awake() {
        _playerInputActions= new PlayerInputActions();
        _playerInputActions.Player.Enable();
    }

    private void OnEnable() {
        _playerInputActions.Player.Interact.performed += InteractOnPerformed;
    }

    private void OnDisable() {
        _playerInputActions.Player.Interact.performed -= InteractOnPerformed;
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke();
    }

    public Vector2 GetMovementNormalizedVector() {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }        
}