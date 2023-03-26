using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TestingNetcodeUI : BaseUI {
        [SerializeField] private Button startHostBtn;
        [SerializeField] private Button startClientBtn;

        private void Awake() {
            startHostBtn.onClick.AddListener(() => {
                NetworkManager.Singleton.StartHost();
                Hide();
            });
            startClientBtn.onClick.AddListener(() => {
                NetworkManager.Singleton.StartClient();
                Hide();
            });
        }
    }
}