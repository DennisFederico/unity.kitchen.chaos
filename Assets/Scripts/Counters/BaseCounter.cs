using System;
using KitchenObjects;
using UnityEngine;

namespace Counters {
    public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent {

        public static event EventHandler AnyItemPlaced;
        [SerializeField] private Transform kitchenObjectParentPoint;
        private KitchenObject _kitchenObject;

        public abstract void Interact(Player.Player player);
    
        public virtual void InteractAlternate(Player.Player player) {
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
            if (kitchenObject) {
                AnyItemPlaced?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ClearKitchenObject() {
            _kitchenObject = null;
        }

        public bool HasKitchenObject() {
            return _kitchenObject != null;
        }
    }
}