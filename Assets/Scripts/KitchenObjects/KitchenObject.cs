using ScriptableObjects;
using UnityEngine;

namespace KitchenObjects {
    public class KitchenObject : MonoBehaviour {
        [field: SerializeField] public KitchenObjectScriptable KitchenObjectScriptable { private set; get; }

        private IKitchenObjectParent _kitchenObjectParent;

        public static KitchenObject SpawnKitchenObject(KitchenObjectScriptable kitchenObjectScriptable, IKitchenObjectParent kitchenObjectParent) {
            var kitchenObjectInstance = Instantiate(kitchenObjectScriptable.prefab);
            var kitchenObject = kitchenObjectInstance.GetComponent<KitchenObject>();
            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
            return kitchenObject;
        }
    
        public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
            if (_kitchenObjectParent != null) {
                _kitchenObjectParent.ClearKitchenObject();
            }

            if (kitchenObjectParent.HasKitchenObject()) {
                Debug.Log($"ERROR IKitchenObjectParent {kitchenObjectParent} already has a KitchenObject. Cannot set {this}");
            }
            _kitchenObjectParent = kitchenObjectParent;
            _kitchenObjectParent.SetKitchenObject(this);
        
            transform.parent = _kitchenObjectParent.GetKitchenObjectParentPoint();
            transform.localPosition = Vector3.zero;
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