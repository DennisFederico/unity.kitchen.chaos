using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ConnectionResponseMessageUI : BaseUI {

        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button closeButton;

        private const string GenericConnectionErrorMessage = "Failed to connect";
        private void Awake() {
            closeButton.onClick.AddListener(Hide);
        }

        private void Start() {
            GameManagerMultiplayer.Instance.FailedToJoinGame += GameManagerMultiplayerOnFailedToJoinGame;
            Hide();
        }
        
        private void OnDestroy() {
            GameManagerMultiplayer.Instance.FailedToJoinGame -= GameManagerMultiplayerOnFailedToJoinGame;
        }

        private void GameManagerMultiplayerOnFailedToJoinGame() {
            messageText.text = string.IsNullOrEmpty(NetworkManager.Singleton.DisconnectReason) ? GenericConnectionErrorMessage : NetworkManager.Singleton.DisconnectReason;
            Show();
        }
    }
}