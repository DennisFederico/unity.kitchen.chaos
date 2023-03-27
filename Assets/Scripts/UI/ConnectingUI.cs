namespace UI {
    public class ConnectingUI : BaseUI {
        private void Start() {
            GameManagerMultiplayer.Instance.TryingToJoinGame += GameManagerMultiplayerOnTryingToJoinGame;
            GameManagerMultiplayer.Instance.FailedToJoinGame += GameManagerMultiplayerOnFailedToJoinGame;
            Hide();
        }
        
        private void OnDestroy() {
            GameManagerMultiplayer.Instance.TryingToJoinGame -= GameManagerMultiplayerOnTryingToJoinGame;
            GameManagerMultiplayer.Instance.FailedToJoinGame -= GameManagerMultiplayerOnFailedToJoinGame;
        }

        private void GameManagerMultiplayerOnFailedToJoinGame() {
            Hide();
        }

        private void GameManagerMultiplayerOnTryingToJoinGame() {
            Show();
        }
    }
}