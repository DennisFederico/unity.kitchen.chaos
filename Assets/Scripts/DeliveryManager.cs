using System;
using System.Collections.Generic;
using System.Linq;
using KitchenObjects;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour {
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private EndRecipeListScriptable levelRecipeList;

    private List<EndRecipeScriptable> _waitingOrders;
    private List<EndRecipeScriptable> _levelRecipesSortedByIngredientName;
    private float _recipeSpawnTimer;
    private float _recipeSpawnTimerMax = 4f;
    private int _waitingRecipesMax = 4;

    private void Awake() {
        Instance = this;
        
        _waitingOrders = new();
        _levelRecipesSortedByIngredientName = new();
        
        //To speed up the comparison process, sort it only once
        foreach (var recipe in levelRecipeList.endRecipesList) {
            var sortedRecipe = ScriptableObject.CreateInstance<EndRecipeScriptable>();
            sortedRecipe.recipeName = recipe.recipeName;
            sortedRecipe.ingredientsList = recipe.ingredientsList.OrderBy(ingredient => ingredient.name).ToList();
            _levelRecipesSortedByIngredientName.Add(sortedRecipe);
        }
    }

    private void Update() {
        _recipeSpawnTimer += Time.deltaTime;
        if (_recipeSpawnTimer > _recipeSpawnTimerMax) {
            _recipeSpawnTimer = 0;
            if (_waitingOrders.Count <= _waitingRecipesMax) {
                var endRecipe = _levelRecipesSortedByIngredientName[Random.Range(0, _levelRecipesSortedByIngredientName.Count)];
                //var endRecipe = levelRecipeList.endRecipesList[Random.Range(0, levelRecipeList.endRecipesList.Count)];
                _waitingOrders.Add(endRecipe);
                Debug.Log($"Generated {endRecipe.recipeName}");
            }
        }
    }

    public void DeliverPlate(PlateKitchenObject plate) {
        //match plate ingredients to waiting recipes
        if (TryFulfillWaitingRecipe(out var waitingRecipeIndex, plate.IngredientsList)) {
            Debug.Log($"Order fulfilled [{waitingRecipeIndex}] {_waitingOrders[waitingRecipeIndex].name}");
            _waitingOrders.RemoveAt(waitingRecipeIndex);
        } else {
            Debug.Log("No recipe fulfilled!!");            
        }
    }

    private bool TryFulfillWaitingRecipe(out int recipeIndex, List<KitchenObjectScriptable> ingredients) {
        for (var i = 0; i < _waitingOrders.Count; i++) {
            if (!CompareIngredientLists(ingredients, _waitingOrders[i].ingredientsList)) continue;
            recipeIndex = i;
            return true;
        }
        recipeIndex = -1;
        return false;
    }
    
    // private readonly Comparison<KitchenObjectScriptable> _ingredientNameComparison =
    //     (ingredient1, ingredient2) => String.Compare(ingredient1.name, ingredient2.name, StringComparison.Ordinal);

    //This comparison would allow duplicate ingredients
    private bool CompareIngredientLists(List<KitchenObjectScriptable> plateIngredients, List<KitchenObjectScriptable> orderIngredients) {
        if (plateIngredients.Count != orderIngredients.Count) return false;
        //Assume orderIngredients to be sorted
        //var list1 = plateIngredients.OrderBy(ingredient => ingredient.name).ToList();
        var list1 = orderIngredients;
        var list2 = plateIngredients.OrderBy(ingredient => ingredient.name).ToList();
        return !list1.Where((t, i) => t != list2[i]).Any();
    }
}