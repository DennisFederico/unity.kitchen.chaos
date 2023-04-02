using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class MainMenuUI : MonoBehaviour {
        [SerializeField] private Button singlePlayerButton;
        [SerializeField] private Button multiplayerButton;
        [SerializeField] private Button quitButton;

        private void Awake() {
            singlePlayerButton.onClick.AddListener(() => {
                GameManagerMultiplayer.playMultiplayer = false;
                Loader.Load(Loader.Scene.LobbyScene);
            });
            multiplayerButton.onClick.AddListener(() => {
                GameManagerMultiplayer.playMultiplayer = true;
                Loader.Load(Loader.Scene.LobbyScene);
            });

            quitButton.onClick.AddListener(Application.Quit);

            Time.timeScale = 1f;
        }
    }
}