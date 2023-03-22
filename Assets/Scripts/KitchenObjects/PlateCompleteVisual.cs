using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace KitchenObjects {
    public class PlateCompleteVisual : MonoBehaviour {
        
        [Serializable]
        public struct ScriptableToPrefabKitchenObject {
            public KitchenObjectScriptable key;
            public GameObject value;
        }
        
        [SerializeField] private PlateKitchenObject plateKitchenObject;

        [SerializeField] private List<ScriptableToPrefabKitchenObject> scriptableToVisualList = new();
        private readonly Dictionary<KitchenObjectScriptable, GameObject> _kitchenObjectScriptableVisualMap = new();
        
        private void Start() {
            plateKitchenObject.AddedIngredient += PlateKitchenObjectOnAddedIngredient;
            foreach (var item in scriptableToVisualList) {
                _kitchenObjectScriptableVisualMap.Add(item.key, item.value);
            }
        }

        private void PlateKitchenObjectOnAddedIngredient(KitchenObjectScriptable ingredient) {
            if (_kitchenObjectScriptableVisualMap.TryGetValue(ingredient, out var ingredientVisual)) {
                ingredientVisual.SetActive(true);
            }
        }
    }
}