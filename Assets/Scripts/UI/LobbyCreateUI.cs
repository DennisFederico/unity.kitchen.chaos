using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class LobbyCreateUI : BaseUI {
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private Toggle isPrivateToggle;
        [SerializeField] private Button createLobbyButton;

        private void Awake() {
            closeButton.onClick.AddListener(Hide);
            createLobbyButton.onClick.AddListener(CreateLobby);
            Hide();
        }

        private void CreateLobby() {
            GameLobby.Instance.CreateLobby(lobbyNameInput.text, isPrivateToggle.isOn);
        }

        public new void Show() {
            base.Show();
        }
    }
}