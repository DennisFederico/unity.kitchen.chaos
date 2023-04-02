using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class DeliveryResultUI : MonoBehaviour {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Color failedColor;
        [SerializeField] private Color successColor;
        [SerializeField] private Sprite failedSprite;
        [SerializeField] private Sprite successSprite;
        [SerializeField] private string successMessage;
        [SerializeField] private string failedMessage;

        private Animator _animator;
        private readonly int _animationPopUpTrigger = Animator.StringToHash("Popup");

        private void Awake() {
            _animator = GetComponent<Animator>();
        }


        private void Start() {
            DeliveryManager.Instance.FailedOrder += DeliveryManagerOnFailedOrder;
            DeliveryManager.Instance.SuccessfulOrder += DeliveryManagerOnSuccessfulOrder;
            gameObject.SetActive(false);
        }

        private void DeliveryManagerOnSuccessfulOrder(Vector3 position) {
            gameObject.SetActive(true);
            backgroundImage.color = successColor;
            iconImage.sprite = successSprite;
            messageText.text = successMessage;
            _animator.SetTrigger(_animationPopUpTrigger);
        }

        private void DeliveryManagerOnFailedOrder(Vector3 position) {
            gameObject.SetActive(true);
            backgroundImage.color = failedColor;
            iconImage.sprite = failedSprite;
            messageText.text = failedMessage;
            _animator.SetTrigger(_animationPopUpTrigger);
        }
    }
}
