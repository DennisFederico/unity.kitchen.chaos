using Counters;
using UnityEngine;

namespace UI {
    public class StoveFlashingBarUI : MonoBehaviour {
        [SerializeField] private StoveCounter stoveCounter;
        private Animator _animator;
        private readonly int _stoveCounterProgressBarFlashing = Animator.StringToHash("IsFlashing");
        private const float BurnShowProgressAmount = .5f;

        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            stoveCounter.OnProgressChange += StoveCounterOnProgressChange;
            _animator.SetBool(_stoveCounterProgressBarFlashing, false);
        }

        private void StoveCounterOnProgressChange(float progress) {
            var show = stoveCounter.IsCooked() && progress >= BurnShowProgressAmount;
            _animator.SetBool(_stoveCounterProgressBarFlashing, show);
        }
    }
}