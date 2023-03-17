using KitchenObjects;
using ScriptableObjects;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour {
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Start() {
        plateKitchenObject.AddedIngredient += PlateKitchenObjectOnAddedIngredient;
    }

    private void PlateKitchenObjectOnAddedIngredient(KitchenObjectScriptable ingredient) {
        UpdateVisual(ingredient);
    }

    private void UpdateVisual(KitchenObjectScriptable ingredient) {
        var iconTransform = Instantiate(iconTemplate, transform);
        iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectScriptable(ingredient);
    }
}