using UnityEngine;

public interface IKitchenObjectParent {
    public Transform GetKitchenObjectParentPoint();

    public KitchenObject GetKitchenObject();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public void ClearKitchenObject();

    public bool HasKitchenObject();
}