using UnityEngine;

public class KitchenObject : MonoBehaviour {
    [field: SerializeField] private KitchenObjectScriptable KitchenObjectScriptable { set; get; }

    private IKitchenObjectParent _kitchenObjectParent;

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

    public IKitchenObjectParent GetKitchenObjectParent() {
        return _kitchenObjectParent;
    }
}