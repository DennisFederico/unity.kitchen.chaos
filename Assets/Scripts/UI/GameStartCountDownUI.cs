using System;
using TMPro;
using UnityEngine;

namespace UI {
    public class GameStartCountDownUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI countDownText;
        private bool _countdown;

        private void Start() {
            GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
            Hide();
        }

        private void Update() {
            if (!_countdown) return;
            countDownText.text = Mathf.CeilToInt(GameManager.Instance.GetCountDownTimer()).ToString();
        }

        private void GameManagerOnGameStateChanged(object sender, EventArgs e) {
            _countdown = GameManager.Instance.IsStartCountDownActive();
            if (_countdown) {
                Show();
            } else {
                Hide();
            }
        }

        private void Hide() {
            countDownText.gameObject.SetActive(false);
        }

        private void Show() {
            countDownText.gameObject.SetActive(true);
        }
    }
}