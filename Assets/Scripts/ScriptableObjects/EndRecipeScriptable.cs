using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu()]
    public class EndRecipeScriptable : ScriptableObject {
        public string recipeName;
        public List<KitchenObjectScriptable> ingredientsList;
    }
}