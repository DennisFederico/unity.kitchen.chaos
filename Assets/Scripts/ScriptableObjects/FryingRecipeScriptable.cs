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
            return recipes.GetFryingRecipeWithInput(input)?.output;
        }

        public static bool HasFryingRecipeWithInput(this FryingRecipeScriptable[] recipes, KitchenObjectScriptable input) {
            return recipes.GetFryingRecipeWithInput(input) != null;
        }
    
        public static bool TryGetFryingRecipeWithInput(this FryingRecipeScriptable[] recipes, out FryingRecipeScriptable output, KitchenObjectScriptable input) {
            output = recipes.GetFryingRecipeWithInput(input);
            return output != null;
        }
        
        public static bool TryGetFryingRecipeIndexWithInput(this FryingRecipeScriptable[] recipes, out int index, KitchenObjectScriptable input) {
            index = -1;
            for (int i = 0; i < recipes.Length; i++) {
                if (recipes[i].input == input) {
                    index = i;
                }
            }
            return index != -1;
        }

        public static FryingRecipeScriptable GetFryingRecipeWithInput(this FryingRecipeScriptable[] recipes, KitchenObjectScriptable input) {
            foreach (var recipe in recipes) {
                if (recipe.input == input) {
                    return recipe;
                }
            }
            return null;
        }
    }
}