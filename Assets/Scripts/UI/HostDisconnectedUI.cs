using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class HostDisconnectedUI : BaseUI {
        [SerializeField] private Button playAgainButton;

        private void Start() {
            NetworkManager.Singleton.OnClientDisconnectCallback += SingletonOnClientDisconnectCallback;
            playAgainButton.onClick.AddListener(() => {
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.MainMenu);
            });
            Hide();
        }

        private void SingletonOnClientDisconnectCallback(ulong clientId) {
            if (clientId == NetworkManager.ServerClientId) {
                Show();
            }
        }

        protected override void Show() {
            base.Show();
            playAgainButton.Select();
        }
    }
}