using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TestingLobbyUI : BaseUI {
        [SerializeField] private Button createGameButton;
        [SerializeField] private Button joinGameButton;

        private void Awake() {
            createGameButton.onClick.AddListener(() => {
                GameManagerMultiplayer.Instance.StartHost();
                Loader.LoadNetwork(Loader.Scene.CharacterSelectionScene);
            });
            joinGameButton.onClick.AddListener(() => {
                GameManagerMultiplayer.Instance.StartClient();
            });
        }
    }
}