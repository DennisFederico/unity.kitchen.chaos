using System;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI {
    public class GameOptionsUI : BaseUI {
        public static GameOptionsUI Instance { get; private set; }

        [SerializeField] private Button musicVolumeButton;
        [SerializeField] private Button soundFxVolumeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI soundFxVolumeText;

        [SerializeField] private Button moveUpButton;
        [SerializeField] private TextMeshProUGUI moveUpText;
        [SerializeField] private Button moveDownButton;
        [SerializeField] private TextMeshProUGUI moveDownText;
        [SerializeField] private Button moveLeftButton;
        [SerializeField] private TextMeshProUGUI moveLeftText;
        [SerializeField] private Button moveRightButton;
        [SerializeField] private TextMeshProUGUI moveRightText;
        [SerializeField] private Button interactButton;
        [SerializeField] private TextMeshProUGUI interactText;

        [FormerlySerializedAs("interactAlt")] [SerializeField]
        private Button interactAltButton;

        [SerializeField] private TextMeshProUGUI interactAltText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private TextMeshProUGUI pauseText;
        [SerializeField] private GameObject pressToRebindMsgModal;

        private Action _onCloseButtonAction;

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
                _onCloseButtonAction();
            });

            moveUpButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveUp));
            moveDownButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveDown));
            moveLeftButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveLeft));
            moveRightButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveRight));
            interactButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Interact));
            interactAltButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.InteractAlt));
            pauseButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Pause));
        }

        private void Start() {
            GameManager.Instance.OnLocalGameUnPaused += LocalGameManagerOnLocalGameUnPaused;
            HideRebindMsg();
            Hide();
        }

        public void Show(Action onClose) {
            _onCloseButtonAction = onClose;
            Show();
        }

        private void LocalGameManagerOnLocalGameUnPaused() {
            Hide();
        }

        private void UpdateVisuals() {
            musicVolumeText.text = $"Music Volume: {Math.Round(MusicManager.Instance.GetVolume() * 10f)}";
            soundFxVolumeText.text = $"SoundFx Volume: {Math.Round(AudioManager.Instance.GetVolume() * 10f)}";

            moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
            moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
            moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
            moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
            interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
            interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
            pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        }

        private void ShowRebindMsg() {
            pressToRebindMsgModal.SetActive(true);
        }

        private void HideRebindMsg() {
            UpdateVisuals();
            pressToRebindMsgModal.SetActive(false);
        }

        private void RebindBinding(GameInput.Binding binding) {
            ShowRebindMsg();
            GameInput.Instance.RebindBinding(binding, HideRebindMsg);
        }

        protected override void Show() {
            base.Show();
            UpdateVisuals();
            closeButton.Select();
        }
    }
}