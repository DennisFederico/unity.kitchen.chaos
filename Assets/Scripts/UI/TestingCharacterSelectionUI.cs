using KitchenLobby;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TestingCharacterSelectionUI : BaseUI {
        [SerializeField] private Button readyButton;

        private void Awake() {
            readyButton.onClick.AddListener(() => CharacterSelectReady.Instance.SetPlayerReady());
        }
    }
}