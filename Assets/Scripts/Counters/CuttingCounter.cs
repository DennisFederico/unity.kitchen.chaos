using System;
using KitchenObjects;
using ScriptableObjects;
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
    
        public override void Interact(Player.Player player) {
            if (!HasKitchenObject() && player.HasKitchenObject()) {
                if (cuttingRecipes.TryGetCuttingRecipeWithInput(out var outputRecipe, player.GetKitchenObject().KitchenObjectScriptable)) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _cuttingProgress = 0;
                    OnProgressChange?.Invoke((float)_cuttingProgress/outputRecipe.cuttingProgressMax);
                }
            } else if (HasKitchenObject()) {
                if (!player.HasKitchenObject()) {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    OnProgressChange?.Invoke(0f);
                } else if (player.GetKitchenObject().TryGetAsPlate(out var plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectScriptable)) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //GetKitchenObject().DestroySelf();
                    }
                }
            }
        }

        public override void InteractAlternate(Player.Player player) {
            if (!HasKitchenObject()) return;
            var kitchenObjectScriptableInput = GetKitchenObject().KitchenObjectScriptable;
            if (cuttingRecipes.TryGetCuttingRecipeWithInput(out var outputRecipe, kitchenObjectScriptableInput)) {
                _cuttingProgress++;
                OnCutAction?.Invoke();
                OnAnyCut?.Invoke(this, EventArgs.Empty);
                OnProgressChange?.Invoke((float) _cuttingProgress/outputRecipe.cuttingProgressMax);
                if (_cuttingProgress >= outputRecipe.cuttingProgressMax) {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    //GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(outputRecipe.output, this);
                }
            }
        }
    }
}