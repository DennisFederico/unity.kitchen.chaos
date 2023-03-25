using System;
using System.Collections.Generic;
using System.Linq;
using Counters;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : NetworkBehaviour {
    public static DeliveryManager Instance { get; private set; }
    public Action newOrderArrived;
    public Action orderFulfilled;
    public event EventHandler SuccessfulOrder;
    public event EventHandler FailedOrder;
    
    [SerializeField] private EndRecipeListScriptable levelRecipeList;
    private int _successfulRecipes;
    private List<EndRecipeScriptable> _waitingOrders;
    private List<EndRecipeScriptable> _levelRecipesSortedByIngredientName;
    private float _recipeSpawnTimer;
    private readonly float _recipeSpawnTimerMax = 4f;
    private readonly int _waitingRecipesMax = 4;

    private void Awake() {
        Instance = this;
        
        _waitingOrders = new();
        _levelRecipesSortedByIngredientName = new();
        
        //To speed up the comparison process, sort it only once - TODO ADD SORT INDEX TO THE INGREDIENT ScriptableObject
        foreach (var recipe in levelRecipeList.endRecipesList) {
            var sortedRecipe = ScriptableObject.CreateInstance<EndRecipeScriptable>();
            sortedRecipe.recipeName = recipe.recipeName;
            sortedRecipe.ingredientsList = recipe.ingredientsList.OrderBy(ingredient => ingredient.name).ToList();
            _levelRecipesSortedByIngredientName.Add(sortedRecipe);
        }
    }

    private void Update() {
        if (!IsServer) return;
        if (!GameManager.Instance.IsGamePlaying()) return;
        _recipeSpawnTimer += Time.deltaTime;
        if (_recipeSpawnTimer > _recipeSpawnTimerMax) {
            _recipeSpawnTimer = 0;
            if (_waitingOrders.Count <= _waitingRecipesMax) {
                var endRecipeIndex = Random.Range(0, _levelRecipesSortedByIngredientName.Count);
                SpawnNewWaitingRecipeClientRpc(endRecipeIndex);
                //var endRecipe = levelRecipeList.endRecipesList[Random.Range(0, levelRecipeList.endRecipesList.Count)];
            }
        }
    }

    //SEND THE RECIPE INDEX AS IT IS EASIER THAT SERIALIZING THE WHOLE THING
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int endRecipeIndex) {
        EndRecipeScriptable endRecipe = _levelRecipesSortedByIngredientName[endRecipeIndex];
        _waitingOrders.Add(endRecipe);
        newOrderArrived?.Invoke();
    }

    public List<EndRecipeScriptable> GetWaitingOrders() {
        return _waitingOrders;
    }
    
    public void DeliverPlate(DeliveryCounter counter, PlateKitchenObject plate) {
        //match plate ingredients to waiting recipes
        if (TryFulfillWaitingRecipe(out var waitingRecipeIndex, plate.IngredientsList)) {
            DeliverCorrectRecipeServerRpc(waitingRecipeIndex);
        } else {
            DeliverIncorrectRecipeServerRpc();
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

    //This comparison would allow duplicate ingredients
    private bool CompareIngredientLists(List<KitchenObjectScriptable> plateIngredients, List<KitchenObjectScriptable> orderIngredients) {
        if (plateIngredients.Count != orderIngredients.Count) return false;
        //Assume orderIngredients to be sorted
        //var list1 = plateIngredients.OrderBy(ingredient => ingredient.name).ToList();
        var list1 = orderIngredients;
        var list2 = plateIngredients.OrderBy(ingredient => ingredient.name).ToList();
        return !list1.Where((t, i) => t != list2[i]).Any();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc() {
        DeliverIncorrectRecipeClientRpc();
    }

    //TODO SHOULD USE THE COUNTER GAMEOBJECT ID 
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc() {
        // FailedOrder?.Invoke(counter, EventArgs.Empty);
        FailedOrder?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeIndex) {
        DeliverCorrectRecipeClientRpc(waitingRecipeIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeIndex) {
        //Here is the actual behavior when a correct recipe is delivered (by any player)
        _successfulRecipes++;
        orderFulfilled?.Invoke();
        _waitingOrders.RemoveAt(waitingRecipeIndex);
        //TODO... here we need the GameObjectId of the counter
        SuccessfulOrder?.Invoke(this, EventArgs.Empty);
    }

    public int GetSuccessfulRecipesAmount() {
        return _successfulRecipes;
    }
}