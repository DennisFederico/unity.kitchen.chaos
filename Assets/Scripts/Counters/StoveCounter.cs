using System;
using KitchenObjects;
using ScriptableObjects;
using UnityEngine;

namespace Counters {
    public class StoveCounter : BaseCounter, IHasProgress {
        
        public event Action<float> OnProgressChange;
        public event Action<bool> StoveOnOffChanged;
        private bool _isTurnedOn = false;
        [SerializeField] private FryingRecipeScriptable[] fryingRecipes;
        private float _currentFryingTime;
        private FryingRecipeScriptable _currentFryingRecipe;
        
        private void Update() {
            if (!HasKitchenObject() || !_isTurnedOn || !_currentFryingRecipe) return;
            _currentFryingTime += Time.deltaTime;
            OnProgressChange?.Invoke(_currentFryingTime/_currentFryingRecipe.maxFryingTime);
            if (_currentFryingTime > _currentFryingRecipe.maxFryingTime) {
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(_currentFryingRecipe.output, this);
                if (fryingRecipes.TryGetCuttingRecipeWithInput(out _currentFryingRecipe, _currentFryingRecipe.output)) {
                    _currentFryingTime = 0;
                    OnProgressChange?.Invoke(0.01f);
                } else {
                    _isTurnedOn = false;
                    StoveOnOffChanged?.Invoke(false);
                    OnProgressChange?.Invoke(1f);
                }
            }
        }

        public override void Interact(Player.Player player) {
            if (!HasKitchenObject() && player.HasKitchenObject()) {
                if (fryingRecipes.TryGetCuttingRecipeWithInput(out var fryingRecipe, player.GetKitchenObject().KitchenObjectScriptable)) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _currentFryingRecipe = fryingRecipe;
                    _currentFryingTime = 0;
                    _isTurnedOn = true;
                    StoveOnOffChanged?.Invoke(true);
                    OnProgressChange?.Invoke(_currentFryingTime/_currentFryingRecipe.maxFryingTime);
                }
            } else if (HasKitchenObject() && !player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                _isTurnedOn = false;
                OnProgressChange?.Invoke(0f);
                StoveOnOffChanged?.Invoke(false);
            }
        }
    }
}
