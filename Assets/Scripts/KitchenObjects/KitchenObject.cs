using System;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace KitchenObjects {
    public class KitchenObject : NetworkBehaviour {
        [field: SerializeField] public KitchenObjectScriptable KitchenObjectScriptable { private set; get; }

        private IKitchenObjectParent _kitchenObjectParent;
        private FollowTransform _followTransform;

        protected virtual void Awake() {
            _followTransform = GetComponent<FollowTransform>();
        }

        public static void SpawnKitchenObject(KitchenObjectScriptable kitchenObjectScriptable, IKitchenObjectParent kitchenObjectParent) {
            GameManagerMultiplayer.Instance.SpawnKitchenObject(kitchenObjectScriptable, kitchenObjectParent);
        }
    
        public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
            SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkReference) {
            SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkReference);
        }
        
        [ClientRpc]
        private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkReference) {
            kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

            _kitchenObjectParent?.ClearKitchenObject();

            if (kitchenObjectParent.HasKitchenObject()) {
                Debug.Log($"ERROR IKitchenObjectParent {kitchenObjectParent} already has a KitchenObject. Cannot set {this}");
            }
            _kitchenObjectParent = kitchenObjectParent;
            _kitchenObjectParent.SetKitchenObject(this);
        
            _followTransform.SetTargetTransform(_kitchenObjectParent.GetKitchenObjectParentPoint());
        }
        
        
        public bool TryGetAsPlate(out PlateKitchenObject plateKitchenObject) {
            if (this is PlateKitchenObject) {
                plateKitchenObject = this as PlateKitchenObject;
                return true;
            }
            plateKitchenObject = null;
            return false;
        }
        
        public IKitchenObjectParent GetKitchenObjectParent() {
            return _kitchenObjectParent;
        }

        public void DestroySelf() {
            _kitchenObjectParent.ClearKitchenObject();
            Destroy(gameObject);
        }
    }
}