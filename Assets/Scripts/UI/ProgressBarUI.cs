using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ProgressBarUI : MonoBehaviour {
        [SerializeField] private GameObject hasProgressCounterGameObject;
        [SerializeField] private Image barImage;
        private IHasProgress _hasProgressCounter;

        private void Start() {
            _hasProgressCounter = hasProgressCounterGameObject.GetComponent<IHasProgress>();
            //On Null pointer exception the GameObject does not implement IHasProgress
            _hasProgressCounter.OnProgressChange += HasProgressCounterOnProgressChange;
            barImage.fillAmount = 0f;
            Hide();
        }

        private void HasProgressCounterOnProgressChange(float amount) {
            barImage.fillAmount = amount;
            if (amount is 0f or >= 1f) {
                Hide();
            } else {
                Show();
            }
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void Show() {
            gameObject.SetActive(true);
        }
    }
}