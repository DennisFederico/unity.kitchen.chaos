using System;
using KitchenObjects;
using KitchenPlayer;
using Unity.Netcode;
using UnityEngine;

namespace Counters {
    public abstract class BaseCounter : NetworkBehaviour, IKitchenObjectParent {

        public static event EventHandler AnyItemPlaced;
        public static void ResetStaticEventHandler() {
            AnyItemPlaced = null;
        }
        [SerializeField] private Transform kitchenObjectParentPoint;
        private KitchenObject _kitchenObject;

        public abstract void Interact(Player player);
    
        public virtual void InteractAlternate(Player player) {
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

        public NetworkObject GetNetworkObject() {
            return NetworkObject;
        }
    }
}