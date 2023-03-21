using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace UI {
    public class OrderUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI recipeName;
        [SerializeField] private Transform ingredientsContainer;
        [SerializeField] private PlateIconSingleUI iconTemplate;


        public void SetRecipeData(EndRecipeScriptable recipe) {
            recipeName.text = recipe.recipeName;
            foreach (var ingredient in recipe.ingredientsList) {
                var ingredientIcon = Instantiate(iconTemplate, ingredientsContainer);
                ingredientIcon.SetKitchenObjectScriptable(ingredient);
            }
        }
    }
}