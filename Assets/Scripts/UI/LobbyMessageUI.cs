using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class LobbyMessageUI : BaseUI {

        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button closeButton;

        private const string GenericConnectionErrorMessage = "Failed to connect";
        private void Awake() {
            closeButton.onClick.AddListener(Hide);
        }

        private void Start() {
            GameManagerMultiplayer.Instance.FailedToJoinGame += GameManagerMultiplayerOnFailedToJoinGame;
            GameLobby.Instance.CreateLobbyStarted += GameLobbyOnCreateLobbyStarted;
            GameLobby.Instance.CreateLobbyFailed += GameLobbyOnCreateLobbyFailed;
            GameLobby.Instance.JoinLobbyStarted += GameLobbyOnJoinLobbyStarted;
            GameLobby.Instance.JoinLobbyFailed += GameLobbyOnJoinLobbyFailed;
            GameLobby.Instance.QuickJoinLobbyFailed += GameLobbyOnQuickJoinLobbyFailed;
            Hide();
        }
        
        private void OnDestroy() {
            GameManagerMultiplayer.Instance.FailedToJoinGame -= GameManagerMultiplayerOnFailedToJoinGame;
            GameLobby.Instance.CreateLobbyStarted -= GameLobbyOnCreateLobbyStarted;
            GameLobby.Instance.CreateLobbyFailed -= GameLobbyOnCreateLobbyFailed;
            GameLobby.Instance.JoinLobbyStarted -= GameLobbyOnJoinLobbyStarted;
            GameLobby.Instance.JoinLobbyFailed -= GameLobbyOnJoinLobbyFailed;
            GameLobby.Instance.QuickJoinLobbyFailed -= GameLobbyOnQuickJoinLobbyFailed;
        }

        private void GameLobbyOnCreateLobbyStarted(object sender, EventArgs e) {
            ShowMessage("Creating Lobby!");
        }
        
        private void GameLobbyOnCreateLobbyFailed(object sender, EventArgs e) {
            ShowMessage("Failed to Create Lobby!");
        }

        private void GameLobbyOnJoinLobbyStarted(object sender, EventArgs e) {
            ShowMessage("Joining Lobby!");
        }
        
        private void GameLobbyOnJoinLobbyFailed(object sender, EventArgs e) {
            ShowMessage("Failed to Join Lobby!");
        }
        
        private void GameLobbyOnQuickJoinLobbyFailed(object sender, EventArgs e) {
            ShowMessage("Could not Find a Lobby to Quick Join");
        }
        
        private void GameManagerMultiplayerOnFailedToJoinGame() {
            messageText.text = string.IsNullOrEmpty(NetworkManager.Singleton.DisconnectReason) ? GenericConnectionErrorMessage : NetworkManager.Singleton.DisconnectReason;
            Show();
        }

        private void ShowMessage(string message) {
            messageText.text = message;
            Show();
        }
    }
}