using System;
using KitchenObjects;
using KitchenPlayer;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counters {
    public class CuttingCounter : BaseCounter, IHasProgress {

        public static event EventHandler OnAnyCut;
        public new static void ResetStaticEventHandler() {
            OnAnyCut = null;
        }
        public event Action<float> OnProgressChange;
        public event Action OnCutAction; 
        [SerializeField] private CuttingRecipeScriptable[] cuttingRecipes;
        private int _cuttingProgress;
    
        public override void Interact(Player player) {
            if (!HasKitchenObject() && player.HasKitchenObject()) {
                if (!cuttingRecipes.HasRecipeWithInput(player.GetKitchenObject().KitchenObjectScriptable)) return;
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);
                InteractLogicPlaceOnCounterServerRpc();
            } else if (HasKitchenObject()) {
                if (!player.HasKitchenObject()) {
                    GetKitchenObject().SetKitchenObjectParent(player);
                } else if (player.GetKitchenObject().TryGetAsPlate(out var plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectScriptable)) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceOnCounterServerRpc() {
            InteractLogicPlaceOnCounterClientRpc();
        }

        [ClientRpc]
        private void InteractLogicPlaceOnCounterClientRpc() {
            _cuttingProgress = 0;
            OnProgressChange?.Invoke(0f);
        }

        public override void InteractAlternate(Player player) {
            if (!HasKitchenObject()) return;
            CutKitchenObjectServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void CutKitchenObjectServerRpc() {
            var kitchenObjectScriptableInput = GetKitchenObject().KitchenObjectScriptable;
            if (!cuttingRecipes.TryGetCuttingRecipeWithInput(out var outputRecipe, kitchenObjectScriptableInput)) return;
            CutKitchenObjectClientRpc();
            if (_cuttingProgress < outputRecipe.cuttingProgressMax) return;
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
            KitchenObject.SpawnKitchenObject(outputRecipe.output, this);
        }

        [ClientRpc]
        private void CutKitchenObjectClientRpc() {
            var kitchenObjectScriptableInput = GetKitchenObject().KitchenObjectScriptable;
            if (!cuttingRecipes.TryGetCuttingRecipeWithInput(out var outputRecipe, kitchenObjectScriptableInput)) return;
            _cuttingProgress++;
            OnCutAction?.Invoke();
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            OnProgressChange?.Invoke((float) _cuttingProgress/outputRecipe.cuttingProgressMax);
        }
    }
}