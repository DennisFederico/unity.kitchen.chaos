using System;
using System.Collections.Generic;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace KitchenObjects {
    public class PlateKitchenObject : KitchenObject {

        public event Action<KitchenObjectScriptable> AddedIngredient;
        //TODO... MAKE IT FOR VALID RECIPES
        [SerializeField] private List<KitchenObjectScriptable> validIngredientsScriptables;
        private List<KitchenObjectScriptable> _ingredientsList;

        protected override void Awake() {
            base.Awake();
            _ingredientsList = new List<KitchenObjectScriptable>();
        }

        //TODO... Here we could follow a recipe as we add ingredients
        public bool TryAddIngredient(KitchenObjectScriptable kitchenObjectScriptable) {
            if (!validIngredientsScriptables.Contains(kitchenObjectScriptable)) return false;
            if (_ingredientsList.Contains(kitchenObjectScriptable)) return false;
            var kitchenObjectScriptableIndex = GameManagerMultiplayer.Instance.GetKitchenObjectScriptableIndex(kitchenObjectScriptable);
            AddIngredientServerRpc(kitchenObjectScriptableIndex);
            return true;
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddIngredientServerRpc(int kitchenObjectScriptableIndex) {
            AddIngredientClientRpc(kitchenObjectScriptableIndex);
        }

        [ClientRpc]
        private void AddIngredientClientRpc(int kitchenObjectScriptableIndex) {
            var kitchenObjectScriptable = GameManagerMultiplayer.Instance.GetKitchenObjectScriptable(kitchenObjectScriptableIndex);
            _ingredientsList.Add(kitchenObjectScriptable);
            AddedIngredient?.Invoke(kitchenObjectScriptable);
        }

        public List<KitchenObjectScriptable> IngredientsList {
            get => _ingredientsList;
            private set => _ingredientsList = value;
        }
    }
}