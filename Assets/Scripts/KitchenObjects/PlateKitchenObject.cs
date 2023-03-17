using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace KitchenObjects {
    public class PlateKitchenObject : KitchenObject {

        public event Action<KitchenObjectScriptable> AddedIngredient;
        //TODO... MAKE IT FOR VALID RECIPES
        [SerializeField] private List<KitchenObjectScriptable> validIngredientsScriptables;
        private List<KitchenObjectScriptable> _ingredientsList;

        private void Awake() {
            _ingredientsList = new List<KitchenObjectScriptable>();
        }

        //TODO... Here we could follow a recipe as we add ingredients
        public bool TryAddIngredient(KitchenObjectScriptable kitchenObjectScriptable) {
            if (!validIngredientsScriptables.Contains(kitchenObjectScriptable)) return false;
            if (_ingredientsList.Contains(kitchenObjectScriptable)) return false;
            
            _ingredientsList.Add(kitchenObjectScriptable);
            AddedIngredient?.Invoke(kitchenObjectScriptable);
            return true;
        }
    }
}