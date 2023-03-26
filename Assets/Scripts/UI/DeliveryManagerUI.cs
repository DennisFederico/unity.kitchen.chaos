using UnityEngine;

namespace UI {
    public class DeliveryManagerUI : BaseUI {

        [SerializeField] private Transform ordersContainer;
        [SerializeField] private OrderUI orderTemplate;

        private void Start() {
            DeliveryManager.Instance.newOrderArrived += DeliveryManagerOnNewOrderArrived;
            DeliveryManager.Instance.orderFulfilled += DeliveryManagerOnOrderFulfilled;
            Hide();
        }

        private void DeliveryManagerOnOrderFulfilled() {
            UpdateOrdersListVisual();
        }

        private void DeliveryManagerOnNewOrderArrived() {
            Show();
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