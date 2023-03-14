using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeScriptable : ScriptableObject {
    public KitchenObjectScriptable input;
    public KitchenObjectScriptable output;
    public int cuttingProgressMax;
}

public static class CuttingRecipeScriptableExtensions {
    public static KitchenObjectScriptable GetKitchenObjectOutputForInput(this CuttingRecipeScriptable[] recipes,  KitchenObjectScriptable input) {
        return recipes.GetCuttingRecipeWithInput(input)?.output;
    }

    public static bool HasRecipeWithInput(this CuttingRecipeScriptable[] recipes, KitchenObjectScriptable input) {
        return recipes.GetCuttingRecipeWithInput(input) != null;
    }
    
    public static bool TryGetCuttingRecipeWithInput(this CuttingRecipeScriptable[] recipes, out CuttingRecipeScriptable output, KitchenObjectScriptable input) {
        output = recipes.GetCuttingRecipeWithInput(input);
        return output != null;
    }
    
    public static CuttingRecipeScriptable GetCuttingRecipeWithInput(this CuttingRecipeScriptable[] recipes, KitchenObjectScriptable input) {
        foreach (var recipe in recipes) {
            if (recipe.input == input) {
                return recipe;
            }
        }
        return null;
    }
}
