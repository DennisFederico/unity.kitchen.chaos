using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class CharacterColorSelectionSingleUI : MonoBehaviour {
        [SerializeField] private Image colorImage;
        [SerializeField] private GameObject selectedMarker;
        private int _colorIndex;

        private void Awake() {
            GetComponent<Button>().onClick.AddListener(() => {
                GameManagerMultiplayer.Instance.ChangePlayerColor(_colorIndex);
            });
        }

        public void SetColor(int index, Color color) {
            _colorIndex = index;
            colorImage.color = color;
        }

        public void MarkSelected() {
            selectedMarker.SetActive(true);
        }

        public void UnMarkSelected() {
            selectedMarker.SetActive(false);
        }
    }
}