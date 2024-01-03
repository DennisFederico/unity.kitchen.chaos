using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public void OnPointerEnter(PointerEventData eventData) {
            Debug.Log($"OnHoverEnter");
        }

        public void OnPointerExit(PointerEventData eventData) {
            Debug.Log($"OnHoverExit");
        }
    }
}