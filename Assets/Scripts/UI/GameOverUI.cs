using System;
using TMPro;
using UnityEngine;

namespace UI {
    public class GameOverUI : MonoBehaviour {
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private TextMeshProUGUI recipesDeliveredText;

        private void Start() {
            GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
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

        private void Hide() {
            gameOverUI.SetActive(false);
        }

        private void Show() {
            gameOverUI.SetActive(true);
        }
    }
}