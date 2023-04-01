using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class LobbyListSingleUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI text;

        private Lobby _lobby;

        private void Awake() {
            GetComponent<Button>().onClick.AddListener(() => {
                GameLobby.Instance.JoinWithId(_lobby.Id);
            });
        }

        public void SetLobby(Lobby lobby) {
            _lobby = lobby;
            text.text = lobby.Name;
        }
    }
}