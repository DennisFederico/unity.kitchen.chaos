using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu()]
    public class AudioClipReferencesScriptable : ScriptableObject {
        public AudioClip[] chop;
        public AudioClip[] deliveryFail;
        public AudioClip[] deliverySuccess;
        public AudioClip[] footstep;
        public AudioClip[] objectDrop;
        public AudioClip[] objectPickup;
        public AudioClip[] trashObject;
        public AudioClip[] warning;
        public AudioClip panSizzle;
    }
}