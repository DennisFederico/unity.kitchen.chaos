using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class GameOverUI : BaseUI {
        [SerializeField] private TextMeshProUGUI recipesDeliveredText;
        [SerializeField] private Button playAgainButton;

        private void Start() {
            GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
            playAgainButton.onClick.AddListener(() => {
                NetworkManager.Singleton.Shutdown();
                Loader.Load(Loader.Scene.MainMenu);
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
            playAgainButton.Select();
        }
    }
}