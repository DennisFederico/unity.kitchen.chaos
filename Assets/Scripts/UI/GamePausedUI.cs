using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class GamePausedUI : MonoBehaviour {

        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake() {
            resumeButton.onClick.AddListener(() => {
                GameManager.Instance.TogglePauseGame();
            });
            mainMenuButton.onClick.AddListener(() => {
                Loader.Load(Loader.Scene.MainMenu);
            });
        }

        private void Start() {
            GameManager.Instance.OnGamePaused += Show;
            GameManager.Instance.OnGameUnPaused += Hide;
            Hide();
        }

        private void Show() {
            gameObject.SetActive(true);
        }
        
        private void Hide() {
            gameObject.SetActive(false);
        }
    }
}