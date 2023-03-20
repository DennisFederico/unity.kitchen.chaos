using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class GameInput : MonoBehaviour {
        private const string ConstPlayerPrefsBindings = "KeyBindings";
        public static GameInput Instance { private set; get; }

        public event Action OnInteractAction;
        public event Action OnInteractAlternateAction;
        public event Action OnPauseAction;
        public event Action OnBindingRebind;

        public enum Binding {
            MoveUp,
            MoveDown,
            MoveLeft,
            MoveRight,
            Interact,
            InteractAlt,
            Pause,
            GamePadInteract,
            GamePadInteractAlt,
            GamePadPause,
        }

        private PlayerInputActions _playerInputActions;

        private void Awake() {
            Instance = this;

            _playerInputActions = new PlayerInputActions();
            
            if (PlayerPrefs.HasKey(ConstPlayerPrefsBindings)) {
                _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(ConstPlayerPrefsBindings));
            }
            
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
            var inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
            return inputVector;
        }

        public string GetBindingText(Binding binding) {
            switch (binding) {
                default:
                case Binding.Interact:
                    return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
                case Binding.InteractAlt:
                    return _playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
                case Binding.Pause:
                    return _playerInputActions.Player.Pause.bindings[0].ToDisplayString();
                case Binding.MoveUp:
                    return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
                case Binding.MoveDown:
                    return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
                case Binding.MoveLeft:
                    return _playerInputActions.Player.Move.bindings[3].ToDisplayString();
                case Binding.MoveRight:
                    return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
                case Binding.GamePadInteract:
                    return _playerInputActions.Player.Interact.bindings[1].ToDisplayString();
                case Binding.GamePadInteractAlt:
                    return _playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
                case Binding.GamePadPause:
                    return _playerInputActions.Player.Pause.bindings[1].ToDisplayString();
            }
        }

        public void RebindBinding(Binding binding, Action onRebound) {
            _playerInputActions.Player.Disable();

            InputAction inputAction;
            int bindingIndex;
            switch (binding) {
                default:
                case Binding.Interact:
                    inputAction = _playerInputActions.Player.Interact;
                    bindingIndex = 0;
                    break;
                case Binding.InteractAlt:
                    inputAction = _playerInputActions.Player.InteractAlternate;
                    bindingIndex = 0;
                    break;
                case Binding.Pause:
                    inputAction = _playerInputActions.Player.Pause;
                    bindingIndex = 0;
                    break;
                case Binding.GamePadInteract:
                    inputAction = _playerInputActions.Player.Interact;
                    bindingIndex = 1;
                    break;
                case Binding.GamePadInteractAlt:
                    inputAction = _playerInputActions.Player.InteractAlternate;
                    bindingIndex = 1;
                    break;
                case Binding.GamePadPause:
                    inputAction = _playerInputActions.Player.Pause;
                    bindingIndex = 1;
                    break;
                case Binding.MoveUp:
                    inputAction = _playerInputActions.Player.Move;
                    bindingIndex = 1;
                    break;
                case Binding.MoveDown:
                    inputAction = _playerInputActions.Player.Move;
                    bindingIndex = 2;
                    break;
                case Binding.MoveLeft:
                    inputAction = _playerInputActions.Player.Move;
                    bindingIndex = 3;
                    break;
                case Binding.MoveRight:
                    inputAction = _playerInputActions.Player.Move;
                    bindingIndex = 4;
                    break;
            }
            

            inputAction.PerformInteractiveRebinding(bindingIndex)
                .OnComplete(callback => {
                    callback.Dispose();
                    _playerInputActions.Player.Enable();
                    onRebound();

                    var bindigsOverride = _playerInputActions.SaveBindingOverridesAsJson();
                    PlayerPrefs.SetString(ConstPlayerPrefsBindings, bindigsOverride);
                    PlayerPrefs.Save();
                    
                    OnBindingRebind?.Invoke();
                }).Start();
        }
    }
}