using Counters;
using UnityEngine;

public class ResetStaticData : MonoBehaviour {
    private void Start() {
        BaseCounter.ResetStaticEventHandler();
        CuttingCounter.ResetStaticEventHandler();
        TrashCounter.ResetStaticEventHandler();
    }
}