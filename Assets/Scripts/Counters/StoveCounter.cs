using System;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counters {
    public class StoveCounter : BaseCounter, IHasProgress {
        
        public event Action<float> OnProgressChange;
        public event Action<bool> StoveOnOffChanged;
        private bool _isTurnedOn;
        [SerializeField] private FryingRecipeScriptable[] fryingRecipes;
        [SerializeField] private KitchenObjectScriptable cookedReference;
        private float _currentFryingTime;
        private FryingRecipeScriptable _currentFryingRecipe;
        
        private void Update() {
            if (!HasKitchenObject() || !_isTurnedOn || !_currentFryingRecipe) return;
            _currentFryingTime += Time.deltaTime;
            OnProgressChange?.Invoke(_currentFryingTime/_currentFryingRecipe.maxFryingTime);
            if (_currentFryingTime > _currentFryingRecipe.maxFryingTime) {
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                //GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(_currentFryingRecipe.output, this);
                if (fryingRecipes.TryGetFryingRecipeWithInput(out _currentFryingRecipe, _currentFryingRecipe.output)) {
                    _currentFryingTime = 0;
                    OnProgressChange?.Invoke(0.01f);
                } else {
                    TurnOffStove();
                }
            }
        }

        public override void Interact(Player.Player player) {
            if (!HasKitchenObject() && player.HasKitchenObject()) {
                if (fryingRecipes.TryGetFryingRecipeIndexWithInput(out var index, player.GetKitchenObject().KitchenObjectScriptable)) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    InteractLogicPlaceOnCounterServerRpc(index);
                }
            } else if (HasKitchenObject()) {
                if (!player.HasKitchenObject()) {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    InteractLogicRemoveFromCounterServerRpc();
                } else if (player.GetKitchenObject().TryGetAsPlate(out var plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectScriptable)) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        InteractLogicRemoveFromCounterServerRpc();
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceOnCounterServerRpc(int index) {
            InteractLogicPlaceOnCounterClientRpc(index);
        }

        [ClientRpc]
        private void InteractLogicPlaceOnCounterClientRpc(int index) {
            _currentFryingRecipe = fryingRecipes[index];
            _currentFryingTime = 0;
            _isTurnedOn = true;
            StoveOnOffChanged?.Invoke(true);
            OnProgressChange?.Invoke(_currentFryingTime/_currentFryingRecipe.maxFryingTime);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicRemoveFromCounterServerRpc() {
            InteractLogicRemoveFromCounterClientRpc();
        }

        [ClientRpc]
        private void InteractLogicRemoveFromCounterClientRpc() {
            TurnOffStove();
        }
        
        private void TurnOffStove() {
            _isTurnedOn = false;
            _currentFryingRecipe = null;
            StoveOnOffChanged?.Invoke(false);
            OnProgressChange?.Invoke(0f);
        }

        public bool IsCooked() {
            return HasKitchenObject() && GetKitchenObject().KitchenObjectScriptable == cookedReference;
        }
    }
}
