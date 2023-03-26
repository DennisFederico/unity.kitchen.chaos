using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class GamePausedUI : BaseUI {

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
            GameManager.Instance.OnLocalGamePaused += Show;
            GameManager.Instance.OnLocalGameUnPaused += Hide;
            Hide();
        }

        protected override void Show() {
            base.Show();
            resumeButton.Select();
        }
    }
}