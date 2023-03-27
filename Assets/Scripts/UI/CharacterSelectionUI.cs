using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class CharacterSelectionUI : BaseUI {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button readyButton;

        private void Awake() {
            mainMenuButton.onClick.AddListener(() => {
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.MainMenu);
            });
            readyButton.onClick.AddListener(() => {
                CharacterSelectReady.Instance.SetPlayerReady();
            });
        }
    }
}