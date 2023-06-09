using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu()]
    public class KitchenObjectScriptable : ScriptableObject {
        public string objectName;
        public Transform prefab;
        public Sprite sprite;
    }
}