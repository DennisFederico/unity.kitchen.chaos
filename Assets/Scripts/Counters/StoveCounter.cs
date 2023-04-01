using System;
using KitchenObjects;
using KitchenPlayer;
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
        private readonly NetworkVariable<float> _currentFryingTime = new();
        private FryingRecipeScriptable _currentFryingRecipe;

        public override void OnNetworkSpawn() {
            _currentFryingTime.OnValueChanged += (_, newValue) => {
                var maxTime = _currentFryingRecipe != null ? _currentFryingRecipe.maxFryingTime : 1f;
                OnProgressChange?.Invoke(newValue/maxTime);
            };
        }

        private void Update() {
            if (!IsServer) return;
            if (!HasKitchenObject() || !_isTurnedOn || !_currentFryingRecipe) return;
            _currentFryingTime.Value += Time.deltaTime;
            //Handle progress by Network Variable
            if (_currentFryingTime.Value > _currentFryingRecipe.maxFryingTime) {
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                KitchenObject.SpawnKitchenObject(_currentFryingRecipe.output, this);
                if (fryingRecipes.TryGetFryingRecipeWithInput(out _currentFryingRecipe, _currentFryingRecipe.output)) {
                    _currentFryingTime.Value = 0f;
                } else {
                    TurnOffStoveServerRpc();
                }
            }
        }

        public override void Interact(Player player) {
            if (!HasKitchenObject() && player.HasKitchenObject()) {
                if (fryingRecipes.TryGetFryingRecipeIndexWithInput(out var index, player.GetKitchenObject().KitchenObjectScriptable)) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    InteractLogicPlaceOnCounterServerRpc(index);
                }
            } else if (HasKitchenObject()) {
                if (!player.HasKitchenObject()) {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    TurnOffStoveServerRpc();
                } else if (player.GetKitchenObject().TryGetAsPlate(out var plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectScriptable)) {
                        TurnOffStoveServerRpc();
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceOnCounterServerRpc(int index) {
            _currentFryingTime.Value = 0;
            InteractLogicPlaceOnCounterClientRpc(index);
        }

        [ClientRpc]
        private void InteractLogicPlaceOnCounterClientRpc(int index) {
            _currentFryingRecipe = fryingRecipes[index];
            _isTurnedOn = true;
            StoveOnOffChanged?.Invoke(_isTurnedOn);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void TurnOffStoveServerRpc() {
            _currentFryingTime.Value = 0f;
            TurnOffStoveClientRpc();
        }

        [ClientRpc]
        private void TurnOffStoveClientRpc() {
            TurnOffStove();
        }
        
        private void TurnOffStove() {
            _currentFryingRecipe = null;
            _isTurnedOn = false;
            StoveOnOffChanged?.Invoke(_isTurnedOn);
        }

        public bool IsCooked() {
            return HasKitchenObject() && GetKitchenObject().KitchenObjectScriptable == cookedReference;
        }
    }
}
