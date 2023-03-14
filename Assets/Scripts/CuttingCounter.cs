using System;
using UnityEngine;

public class CuttingCounter : BaseCounter {

    public event Action<float> OnProgressChange;
    public event Action OnCutAction; 
    [SerializeField] private CuttingRecipeScriptable[] cuttingRecipes;
    private int _cuttingProgress;
    
    public override void Interact(Player player) {
        if (!HasKitchenObject() && player.HasKitchenObject()) {
            if (cuttingRecipes.TryGetCuttingRecipeWithInput(out var outputRecipe, player.GetKitchenObject().KitchenObjectScriptable)) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _cuttingProgress = 0;
                OnProgressChange?.Invoke((float)_cuttingProgress/outputRecipe.cuttingProgressMax);
            }
        } else if (HasKitchenObject() && !player.HasKitchenObject()) {
            GetKitchenObject().SetKitchenObjectParent(player);
            OnProgressChange?.Invoke(0f);
        }
    }

    public override void InteractAlternate(Player player) {
        if (!HasKitchenObject()) return;
        var kitchenObjectScriptableInput = GetKitchenObject().KitchenObjectScriptable;
        if (cuttingRecipes.TryGetCuttingRecipeWithInput(out var outputRecipe, kitchenObjectScriptableInput)) {
            _cuttingProgress++;
            OnCutAction?.Invoke();
            OnProgressChange?.Invoke((float) _cuttingProgress/outputRecipe.cuttingProgressMax);
            if (_cuttingProgress >= outputRecipe.cuttingProgressMax) {
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputRecipe.output, this);
            }
        }
    }
}