using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class LobbyUI : BaseUI {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_InputField playerNameInput;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button quickJoinButton;
        [SerializeField] private LobbyCreateUI lobbyCreateUI;
        [SerializeField] private TMP_InputField lobbyCodeInputField;
        [SerializeField] private Button joinWithCodeButton;
        [SerializeField] private Transform lobbyListContainer;
        [SerializeField] private Transform lobbyListItemTemplate;
        

        private void Awake() {
            mainMenuButton.onClick.AddListener(() => {
                GameLobby.Instance.LeaveLobby();
                Loader.Load(Loader.Scene.MainMenu);
            });
            createLobbyButton.onClick.AddListener(() => lobbyCreateUI.Show());
            quickJoinButton.onClick.AddListener(() => GameLobby.Instance.QuickJoin());
            joinWithCodeButton.onClick.AddListener(() => GameLobby.Instance.JoinWithCode(lobbyCodeInputField.text));
            if (GameManagerMultiplayer.playMultiplayer) {
                Show();
            }
        }

        private void Start() {
            playerNameInput.text = GameManagerMultiplayer.Instance.PlayerName;
            playerNameInput.onValueChanged.AddListener(playerName => GameManagerMultiplayer.Instance.PlayerName = playerName);
            GameLobby.Instance.LobbyListChanged += UpdateLobbyList;
            UpdateLobbyList(new List<Lobby>());
        }

        private void OnDestroy() {
            GameLobby.Instance.LobbyListChanged -= UpdateLobbyList;
        }

        private void UpdateLobbyList(List<Lobby> lobbies) {
            foreach (Transform child in lobbyListContainer) {
                Destroy(child.gameObject);
            }
            foreach (var lobby in lobbies) {
                var lobbyItem = Instantiate(lobbyListItemTemplate, lobbyListContainer);
                lobbyItem.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
            }
        }
    }
}