using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {

    [SerializeField] private Transform kitchenObjectParentPoint;
    private KitchenObject _kitchenObject;

    public virtual void Interact(Player player) {
        Debug.Log($"BaseCounter.Interact - Invalid call");
    }
    
    public virtual void InteractAlternate(Player player) {
        Debug.Log($"BaseCounter.AlternateInteract - Invalid call");
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