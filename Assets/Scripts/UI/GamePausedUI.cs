using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class GamePausedUI : MonoBehaviour {
        [SerializeField] private GameObject gamePausedUI;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake() {
            resumeButton.onClick.AddListener(() => { GameManager.Instance.TogglePauseGame(); });
            optionsButton.onClick.AddListener(() => {
                Hide();
                GameOptionsUI.Instance.Show(Show);
            });
            mainMenuButton.onClick.AddListener(() => { Loader.Load(Loader.Scene.MainMenu); });
        }

        private void Start() {
            GameManager.Instance.OnGamePaused += Show;
            GameManager.Instance.OnGameUnPaused += Hide;
            Hide();
        }

        private void Show() {
            gamePausedUI.SetActive(true);
            resumeButton.Select();
        }

        private void Hide() {
            gamePausedUI.SetActive(false);
        }
    }
}