using UnityEngine;

namespace Counters {
    public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent {

        [SerializeField] private Transform kitchenObjectParentPoint;
        private KitchenObject _kitchenObject;

        public abstract void Interact(Player player);
    
        public virtual void InteractAlternate(Player player) {
            //Debug.Log($"BaseCounter.AlternateInteract - Invalid call");
        }

        public Transform GetKitchenObjectParentPoint() {
            return kitchenObjectParentPoint;
        }

        public KitchenObject GetKitchenObject() {
            return _kitchenObject;
        }

        public void SetKitchenObject(KitchenObject kitchenObject) {
            _kitchenObject = kitchenObject;
        }

        public void ClearKitchenObject() {
            _kitchenObject = null;
        }

        public bool HasKitchenObject() {
            return _kitchenObject != null;
        }
    }
}