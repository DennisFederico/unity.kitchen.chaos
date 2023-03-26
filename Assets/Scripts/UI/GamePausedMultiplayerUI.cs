namespace UI {
    public class GamePausedMultiplayerUI : BaseUI {
        private void Start() {
            GameManager.Instance.OnMultiplayerGamePaused += Show;
            GameManager.Instance.OnMultiplayerGameUnPaused += Hide;
            Hide();
        }
    }
}