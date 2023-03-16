using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace KitchenObjects {
    public class PlateKitchenObject : KitchenObject {

        private List<KitchenObjectScriptable> _ingredientsList;

        private void Awake() {
            _ingredientsList = new List<KitchenObjectScriptable>();
        }

        public void AddIngredient(KitchenObjectScriptable kitchenObjectScriptable) {
            _ingredientsList.Add(kitchenObjectScriptable);
        }
    }
}