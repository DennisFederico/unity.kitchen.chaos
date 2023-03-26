using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class GameOverUI : BaseUI {
        [SerializeField] private TextMeshProUGUI recipesDeliveredText;
        [SerializeField] private Button retryButton;

        private void Start() {
            GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
            retryButton.onClick.AddListener(() => {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
            Hide();
        }

        private void GameManagerOnGameStateChanged(object sender, EventArgs e) {
            if (GameManager.Instance.IsGameOver()) {
                recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
                Show();
            } else {
                Hide();
            }
        }

        protected override void Show() {
            base.Show();
            retryButton.Select();
        }
    }
}