using System;
using KitchenPlayer;
using UnityEngine;

namespace Counters {
    public class CounterSelected : MonoBehaviour {

        [SerializeField] private BaseCounter baseCounter;
        [SerializeField] private GameObject[] selectedVisualArray;

        private void Start() {
            if (Player.LocalInstance != null) {
                Player.LocalInstance.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;                
            } else {
                Player.localPlayerSpawned += AnyPlayerSpawned;
            }
        }

        private void AnyPlayerSpawned(object sender, EventArgs e) {
            if (Player.LocalInstance != null) {
                Player.LocalInstance.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;                
            }
        }
        
        private void PlayerOnSelectedCounterChanged(Player.SelectedCounter counterSelectedChange) {
            if (baseCounter == counterSelectedChange.baseCounter) {
                Selected();
            } else {
                //THIS COULD BE OPTIMIZED BY CHECKING IF IT WAS SELECTED FIRST
                //OR SENDING NEW AND PREVIOUS SELECTION ON THE EVENT
                UnSelected();
            }
        }

        private void Selected() {
            foreach (var selectedVisual in selectedVisualArray) {
                selectedVisual.SetActive(true);
            }
        }
    
        private void UnSelected() {
            foreach (var selectedVisual in selectedVisualArray) {
                selectedVisual.SetActive(false);
            }
        }
    }
}