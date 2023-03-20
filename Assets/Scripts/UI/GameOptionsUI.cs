using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class GameOptionsUI : MonoBehaviour {
        
        public static GameOptionsUI Instance { get; private set; }
        
        [SerializeField] private GameObject gameOptionsUI;
        [SerializeField] private Button musicVolumeButton;
        [SerializeField] private Button soundFxVolumeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI soundFxVolumeText;

        private void Awake() {
            Instance = this;
            musicVolumeButton.onClick.AddListener(() => {
                MusicManager.Instance.ChangeVolume();
                UpdateVisuals();
            });
            soundFxVolumeButton.onClick.AddListener(() => {
                AudioManager.Instance.ChangeVolume();
                UpdateVisuals();
            });
            closeButton.onClick.AddListener(() => {
                Hide();
            });
        }

        private void Start() {
            GameManager.Instance.OnGameUnPaused += GameManagerOnGameUnPaused;
            Hide();
        }

        public void Show() {
            UpdateVisuals();
            gameOptionsUI.SetActive(true);
        }

        public void Hide() {
            gameOptionsUI.SetActive(false);
        }
        
        private void GameManagerOnGameUnPaused() {
            Hide();
        }

        private void UpdateVisuals() {
            musicVolumeText.text = $"Music Volume: {Math.Round(MusicManager.Instance.GetVolume() * 10f)}";
            soundFxVolumeText.text = $"SoundFx Volume: {Math.Round(AudioManager.Instance.GetVolume() * 10f)}";
        }
    }
}