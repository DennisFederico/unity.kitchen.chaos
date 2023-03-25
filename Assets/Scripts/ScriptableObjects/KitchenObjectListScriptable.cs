using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu()]
    public class KitchenObjectListScriptable : ScriptableObject {
        public List<KitchenObjectScriptable> kitchenObjects;
    }
}