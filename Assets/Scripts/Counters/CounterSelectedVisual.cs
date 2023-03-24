using UnityEngine;

namespace Counters {
    public class CounterSelected : MonoBehaviour {

        [SerializeField] private BaseCounter baseCounter;
        [SerializeField] private GameObject[] selectedVisualArray;

        private void Start() {
            //Player.Player.Instance.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;
        }
    
        // private void OnEnable() {
        //     Player.Instance.OnSelectedCounterChanged += PlayerOnOnSelectedCounterChanged;
        // }

        // private void OnDisable() {
        //     Player.Instance.OnSelectedCounterChanged -= PlayerOnOnSelectedCounterChanged;
        // }

        private void PlayerOnSelectedCounterChanged(Player.Player.SelectedCounter counterSelectedChange) {
            if (baseCounter == counterSelectedChange.BaseCounter) {
                Selected();
            } else {
                UnSelected();
            }
        }

        private void Selected() {
            foreach (var selectedVisual in selectedVisualArray) {
                selectedVisual.SetActive(true);
            }
        }
    
        private void UnSelected() {
            foreach (var selectedVisual in selectedVisualArray) {
                selectedVisual.SetActive(false);
            }
        }
    }
}