using ScriptableObjects;
using UnityEngine;

namespace UI {
    public class DeliveryManagerUI : MonoBehaviour {
        [SerializeField] private Transform ordersContainer;
        [SerializeField] private OrderUI orderTemplate;

        private void Start() {
            DeliveryManager.Instance.newOrderArrived += DeliveryManagerOnNewOrderArrived;
            DeliveryManager.Instance.orderFulfilled += DeliveryManagerOnOrderFulfilled;
        }

        private void DeliveryManagerOnOrderFulfilled() {
            UpdateOrdersListVisual();
        }

        private void DeliveryManagerOnNewOrderArrived() {
            UpdateOrdersListVisual();
        }

        private void UpdateOrdersListVisual() {
            var orders = DeliveryManager.Instance.GetWaitingOrders();
            foreach (Transform child in ordersContainer.transform) {
                Destroy(child.gameObject);
            }

            foreach (var order in orders) {
                var orderVisual = Instantiate(orderTemplate, ordersContainer);
                orderVisual.SetRecipeData(order);
                orderVisual.gameObject.SetActive(true);
            }
        }
    }
}