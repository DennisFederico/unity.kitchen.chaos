using UnityEngine;

namespace UI {
    public class BaseUI : MonoBehaviour {
        [SerializeField] protected GameObject gameObjectUI;
        
        protected virtual void Hide() {
            gameObjectUI.SetActive(false);
        }

        protected virtual void Show() {
            gameObjectUI.SetActive(true);
        }
    }
}