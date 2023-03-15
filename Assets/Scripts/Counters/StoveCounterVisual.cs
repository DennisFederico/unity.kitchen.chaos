using System;
using UnityEngine;

namespace Counters {
    public class StoveCounterVisual : MonoBehaviour {
        [SerializeField] private StoveCounter stoveCounter;
        [SerializeField] private GameObject stoveOnGameObject;
        [SerializeField] private GameObject particlesGameObject;

        private void OnEnable() {
            stoveCounter.StoveOnOffChanged += StoveCounterOnStoveOnOffChanged;
        }
        private void OnDisable() {
            stoveCounter.StoveOnOffChanged -= StoveCounterOnStoveOnOffChanged;
        }
        
        private void StoveCounterOnStoveOnOffChanged(bool isOn) {
            stoveOnGameObject.SetActive(isOn);
            particlesGameObject.SetActive(isOn);
        }
    }
}