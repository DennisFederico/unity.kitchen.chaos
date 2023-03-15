using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu()]
    public class FryingRecipeScriptable : ScriptableObject {
        public KitchenObjectScriptable input;
        public KitchenObjectScriptable output;
        public float maxFryingTime;
    }
    
    public static class FryingRecipeScriptableExtensions {
        public static KitchenObjectScriptable GetKitchenObjectOutputForInput(this FryingRecipeScriptable[] recipes,  KitchenObjectScriptable input) {
            return recipes.GetCuttingRecipeWithInput(input)?.output;
        }

        public static bool HasRecipeWithInput(this FryingRecipeScriptable[] recipes, KitchenObjectScriptable input) {
            return recipes.GetCuttingRecipeWithInput(input) != null;
        }
    
        public static bool TryGetCuttingRecipeWithInput(this FryingRecipeScriptable[] recipes, out FryingRecipeScriptable output, KitchenObjectScriptable input) {
            output = recipes.GetCuttingRecipeWithInput(input);
            return output != null;
        }
    
        public static FryingRecipeScriptable GetCuttingRecipeWithInput(this FryingRecipeScriptable[] recipes, KitchenObjectScriptable input) {
            foreach (var recipe in recipes) {
                if (recipe.input == input) {
                    return recipe;
                }
            }
            return null;
        }
    }
}