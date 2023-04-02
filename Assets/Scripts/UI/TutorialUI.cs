using System;
using KitchenPlayer;
using TMPro;
using UnityEngine;

namespace UI {
    public class TutorialUI : BaseUI {

        [SerializeField] private TextMeshProUGUI keyMoveLeft;
        [SerializeField] private TextMeshProUGUI keyMoveUp;
        [SerializeField] private TextMeshProUGUI keyMoveDown;
        [SerializeField] private TextMeshProUGUI keyMoveRight;
        [SerializeField] private TextMeshProUGUI keyInteract;
        [SerializeField] private TextMeshProUGUI keyInteractAlt;
        [SerializeField] private TextMeshProUGUI keyPause;
        [SerializeField] private TextMeshProUGUI keyGamePadInteract;
        [SerializeField] private TextMeshProUGUI keyGamePadInteractAlt;
        [SerializeField] private TextMeshProUGUI keyGamePadPause;

        private void Start() {
            GameInput.Instance.OnBindingRebind += GameInputOnBindingRebind;
            // GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
            GameManager.Instance.LocalPlayerReadyChanged += GameManagerOnLocalPlayerReadyChanged;
            UpdateVisuals();
            if (GameManager.Instance.IsWaitingToStart()) {
                Show();
            } else {
                Hide();
            }
        }

        private void GameManagerOnLocalPlayerReadyChanged(object sender, EventArgs e) {
            if (GameManager.Instance.IsLocalPlayerReady()) {
                Hide();
            }
        }
        
        private void GameInputOnBindingRebind() {
            UpdateVisuals();
        }

        private void UpdateVisuals() {
            keyMoveUp.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
            keyMoveDown.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
            keyMoveLeft.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
            keyMoveRight.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
            keyInteract.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
            keyInteractAlt.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
            keyPause.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);

            keyGamePadInteract.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
            keyGamePadInteractAlt.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
            keyGamePadPause.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
        }
    }
}