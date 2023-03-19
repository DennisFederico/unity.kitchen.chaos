using System;
using TMPro;
using UnityEngine;

namespace UI {
    public class TimeLeftUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI timerText;
        private bool _playing;

        private void Start() {
            GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
            Hide();
        }

        private void Update() {
            if (!_playing) return;
            timerText.text = GameManager.Instance.GetPlayTimeLeft().ToString("#.##");
        }

        private void GameManagerOnGameStateChanged(object sender, EventArgs e) {
            _playing = GameManager.Instance.IsGamePlaying();
            if (_playing) {
                Show();
            } else {
                Hide();
            }
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void Show() {
            gameObject.SetActive(true);
        }
        
    }
}