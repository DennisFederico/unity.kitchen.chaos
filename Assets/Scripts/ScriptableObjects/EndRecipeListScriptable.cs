using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects {
    [CreateAssetMenu()]
    public class EndRecipeListScriptable : ScriptableObject {
        [FormerlySerializedAs("endRecipeList")] public List<EndRecipeScriptable> endRecipesList;
    }
}