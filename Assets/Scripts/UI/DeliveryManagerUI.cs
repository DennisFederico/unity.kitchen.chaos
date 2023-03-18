using ScriptableObjects;
using UnityEngine;

namespace UI {
    public class DeliveryManagerUI : MonoBehaviour {
        [SerializeField] private Transform ordersContainer;
        [SerializeField] private OrderUI orderTemplate;

        private void Start() {
            DeliveryManager.Instance.NewOrderArrived += DeliveryManagerOnNewOrderArrived;
            DeliveryManager.Instance.OrderFulfilled += DeliveryManagerOnOrderFulfilled;
        }

        private void DeliveryManagerOnOrderFulfilled(EndRecipeScriptable recipe) {
            UpdateOrdersListVisual();
        }

        private void DeliveryManagerOnNewOrderArrived(EndRecipeScriptable recipe) {
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