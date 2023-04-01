using KitchenLobby;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class CharacterSelectionUI : BaseUI {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        private void Awake() {
            mainMenuButton.onClick.AddListener(() => {
                GameLobby.Instance.LeaveLobby();
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.MainMenu);
            });
            readyButton.onClick.AddListener(() => {
                CharacterSelectReady.Instance.SetPlayerReady();
            });
        }

        private void Start() {
            var joinedLobby = GameLobby.Instance.GetJoinedLobby();
            lobbyNameText.text = $"Lobby Name: {joinedLobby?.Name}";
            lobbyCodeText.text = $"Lobby Code: {joinedLobby?.LobbyCode}";
        }
    }
}