using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class HostDisconnectedUI : BaseUI {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button playAgainButton;

        private void Start() {
            NetworkManager.Singleton.OnClientDisconnectCallback += SingletonOnClientDisconnectCallback;
            playAgainButton.onClick.AddListener(() => {
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.MainMenu);
            });
            Hide();
        }

        private void OnDestroy() {
            if (NetworkManager.Singleton) {
                NetworkManager.Singleton.OnClientDisconnectCallback -= SingletonOnClientDisconnectCallback;                
            }
        }

        private void SingletonOnClientDisconnectCallback(ulong clientId) {
            if (clientId == NetworkManager.ServerClientId) {
                if (NetworkManager.Singleton.DisconnectReason != string.Empty) {
                    messageText.text = NetworkManager.Singleton.DisconnectReason;
                }
                Show();
            }
        }

        protected override void Show() {
            base.Show();
            playAgainButton.Select();
        }
    }
}