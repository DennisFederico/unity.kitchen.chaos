using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu()]
    public class EndRecipeListScriptable : ScriptableObject {
        public List<EndRecipeScriptable> endRecipesList;
    }
}