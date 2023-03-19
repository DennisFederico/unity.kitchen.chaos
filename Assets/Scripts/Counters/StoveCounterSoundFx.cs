using UnityEngine;

namespace Counters {
    public class StoveCounterSoundFx : MonoBehaviour {
        [SerializeField] private StoveCounter stoveCounter;
        private AudioSource _audioSource;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start() {
            stoveCounter.StoveOnOffChanged += StoveCounterOnStoveOnOffChanged;
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