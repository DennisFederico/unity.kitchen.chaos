using System;

namespace UI {
    public class WaitingForPlayersUI : BaseUI {
        private void Start() {
            GameManager.Instance.LocalPlayerReadyChanged += GameManagerOnLocalPlayerReadyChanged;
            GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void GameManagerOnGameStateChanged(object sender, EventArgs e) {
            if (GameManager.Instance.IsStartCountDownActive()) {
                Hide();
            }
        }

        private void GameManagerOnLocalPlayerReadyChanged(object sender, EventArgs e) {
            if (GameManager.Instance.IsLocalPlayerReady()) {
                Show();
            }
        }
    }
}