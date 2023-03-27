using UnityEngine;

namespace Counters {
    public class StoveCounterSoundFx : MonoBehaviour {
        [SerializeField] private StoveCounter stoveCounter;
        private AudioSource _audioSource;
        private const float BurnShowProgressAmount = .5f;
        private bool _playWarningSound;
        private float _warningSoundTimer;
        private readonly float _warningSoundTimerMax = .2f;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start() {
            stoveCounter.StoveOnOffChanged += StoveCounterOnStoveOnOffChanged;
            stoveCounter.OnProgressChange += StoveCounterOnProgressChange;
        }

        private void Update() {
            if (!_playWarningSound) return;
            _warningSoundTimer -= Time.deltaTime;
            if (_warningSoundTimer < 0f) {
                _warningSoundTimer = _warningSoundTimerMax;
                AudioManager.Instance.PlayStoveWarningSound(stoveCounter.transform.position);
            }
        }

        private void StoveCounterOnProgressChange(float progress) {
            _playWarningSound = stoveCounter.IsCooked() && progress >= BurnShowProgressAmount;
        }

        private void StoveCounterOnStoveOnOffChanged(bool isOn) {
            if (isOn) {
                _audioSource.Play();
            } else {
                _audioSource.Pause();
            }
        }
    }
}