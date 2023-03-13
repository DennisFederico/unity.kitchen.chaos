using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent {

    [SerializeField] private Transform kitchenObjectParentPoint;
    [SerializeField] private KitchenObjectScriptable kitchenObjectScriptable;
    private KitchenObject _kitchenObject;

    public void Interact(Player player) {
        if (_kitchenObject == null) {
            var kitchenObjectInstance = Instantiate(kitchenObjectScriptable.prefab, kitchenObjectParentPoint);
            kitchenObjectInstance.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        } else {
            //Pickup by the player
            _kitchenObject.SetKitchenObjectParent(player);
        }
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