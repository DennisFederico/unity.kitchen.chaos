using AYellowpaper.SerializedCollections;
using ScriptableObjects;
using UnityEngine;

namespace KitchenObjects {
    public class PlateCompleteVisual : MonoBehaviour {
        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private SerializedDictionary<KitchenObjectScriptable, GameObject> kitchenObjectScriptableVisualMap = new();

        private void Start() {
            plateKitchenObject.AddedIngredient += PlateKitchenObjectOnAddedIngredient;
        }

        private void PlateKitchenObjectOnAddedIngredient(KitchenObjectScriptable ingredient) {
            if (kitchenObjectScriptableVisualMap.TryGetValue(ingredient, out var ingredientVisual)) {
                ingredientVisual.SetActive(true);
            }
        }
    }
}