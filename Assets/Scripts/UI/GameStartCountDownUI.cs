using System;
using TMPro;
using UnityEngine;

namespace UI {
    public class GameStartCountDownUI : BaseUI {
        [SerializeField] private TextMeshProUGUI countDownText;
        private Animator _animator;
        private int _previousCount;
        private bool _countdown;
        private readonly int _numberPopupAnimationTrigger = Animator.StringToHash("NumberPopup");

        private void Awake() {
            _animator = gameObjectUI.GetComponent<Animator>();
        }

        private void Start() {
            GameManager.Instance.GameStateChanged += GameManagerOnGameStateChanged;
            Hide();
        }

        private void Update() {
            if (!_countdown) return;
            var currentCount = Mathf.CeilToInt(GameManager.Instance.GetCountDownTimer());
            countDownText.text = currentCount.ToString();
            if (currentCount != _previousCount) {
                _previousCount = currentCount;
                _animator.SetTrigger(_numberPopupAnimationTrigger);
                AudioManager.Instance.PlayCountDownSound();
            }
        }

        private void GameManagerOnGameStateChanged(object sender, EventArgs e) {
            _countdown = GameManager.Instance.IsStartCountDownActive();
            if (_countdown) {
                Show();
            } else {
                Hide();
            }
        }
    }
}