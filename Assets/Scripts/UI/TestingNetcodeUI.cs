using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TestingNetcodeUI : BaseUI {
        [SerializeField] private Button startHostBtn;
        [SerializeField] private Button startClientBtn;

        private void Awake() {
            startHostBtn.onClick.AddListener(() => {
                GameManagerMultiplayer.Instance.StartHost();
                Hide();
            });
            startClientBtn.onClick.AddListener(() => {
                GameManagerMultiplayer.Instance.StartClient();
                Hide();
            });
        }
    }
}