using System;
using ScriptableObjects;
using UnityEngine;

namespace Counters {
    public class StoveCounter : BaseCounter {

        public event Action<bool> StoveOnOffChanged;
        private bool _isTurnedOn = false;
        [SerializeField] private FryingRecipeScriptable[] fryingRecipes;
        private float _currentFryingTime;
        
        private void Update() {
            if (!HasKitchenObject() || !_isTurnedOn) return;
            _currentFryingTime += Time.deltaTime;
            UpdateRecipeState();
        }

        private void UpdateRecipeState() {
            if (fryingRecipes.TryGetCuttingRecipeWithInput(out var fryingRecipe, GetKitchenObject().KitchenObjectScriptable)) {
                if (_currentFryingTime > fryingRecipe.maxFryingTime) {
                    if (fryingRecipes.HasRecipeWithInput(fryingRecipe.input)) {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipe.output, this);
                        _currentFryingTime = 0;
                    }
                }
            } else {
                //Burned? ... TODO smoke effect?
                _isTurnedOn = false;
                StoveOnOffChanged?.Invoke(false);
            }
        }

        public override void Interact(Player player) {
            if (!HasKitchenObject() && player.HasKitchenObject()) {
                if (fryingRecipes.TryGetCuttingRecipeWithInput(out var outputRecipe, player.GetKitchenObject().KitchenObjectScriptable)) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _currentFryingTime = 0;
                    _isTurnedOn = true;
                    StoveOnOffChanged?.Invoke(true);
                }
            } else if (HasKitchenObject() && !player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                _isTurnedOn = false;
                StoveOnOffChanged?.Invoke(false);
            }
        }
    }
}
