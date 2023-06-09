using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TimeLeftUI : MonoBehaviour {
        [SerializeField] private GameObject timeLeftUI;
        [SerializeField] private Image timerImage;
        private bool _playing;

        private void Start() {
            GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
            Hide();
        }

        private void Update() {
            if (!_playing) return;
            timerImage.fillAmount = GameManager.Instance.GetPlayTimeLeftNormalized();
        }

        private void GameManagerOnGameStateChanged(object sender, EventArgs e) {
            _playing = GameManager.Instance.IsGamePlaying();
            if (_playing) Show();
        }

        private void Show() {
            timeLeftUI.SetActive(true);
        }

        private void Hide() {
            timeLeftUI.SetActive(false);
        }
    }
}