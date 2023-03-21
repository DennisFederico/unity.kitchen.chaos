using Counters;
using UnityEngine;

namespace UI {
    public class StoveBurnWarningUI : MonoBehaviour {
        [SerializeField] private StoveCounter stoveCounter;
        private const float BurnShowProgressAmount = .5f;

        private void Start() {
            stoveCounter.OnProgressChange += StoveCounterOnProgressChange;
            Hide();
        }

        private void StoveCounterOnProgressChange(float progress) {
            var show = stoveCounter.IsCooked() && progress >= BurnShowProgressAmount;
            if (show) {
                Show();
            } else {
                Hide();
            }
        }

        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }
    }
}