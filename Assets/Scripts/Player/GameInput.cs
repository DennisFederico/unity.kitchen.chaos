using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class GameInput : MonoBehaviour {
        public static GameInput Instance { private set; get; }

        public event Action OnInteractAction;
        public event Action OnInteractAlternateAction;
        public event Action OnPauseAction;
        private PlayerInputActions _playerInputActions;

        private void Awake() {
            Instance = this;
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();
        }

        private void OnEnable() {
            _playerInputActions.Player.Interact.performed += InteractOnPerformed;
            _playerInputActions.Player.InteractAlternate.performed += InteractAlternateOnPerformed;
            _playerInputActions.Player.Pause.performed += PauseOnPerformed;
        }

        private void PauseOnPerformed(InputAction.CallbackContext obj) {
            OnPauseAction?.Invoke();
        }

        private void OnDisable() {
            _playerInputActions.Player.Interact.performed -= InteractOnPerformed;
            _playerInputActions.Player.InteractAlternate.performed -= InteractAlternateOnPerformed;
            _playerInputActions.Player.Pause.performed -= PauseOnPerformed;
        }

        private void InteractOnPerformed(InputAction.CallbackContext obj) {
            OnInteractAction?.Invoke();
        }

        private void InteractAlternateOnPerformed(InputAction.CallbackContext obj) {
            OnInteractAlternateAction?.Invoke();
        }

        public Vector2 GetMovementNormalizedVector() {
            Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
            return inputVector;
        }
    }
}