using System;
using ScriptableObjects;
using UnityEngine;

namespace Counters {
    public class PlatesCounter : BaseCounter {

        public event Action PlateSpawned;
        public event Action PlateGrabbed;
        [SerializeField] private KitchenObjectScriptable plateKitchenObjectScriptable;
        private float _spawnPlateTimer;
        private float _spawnPlateTimerMax = 4f;
        private int _spawnedPlatesAmount;
        private int _spawnedPlatesMaxAmount = 4;

        private void Update() {
            _spawnPlateTimer += Time.deltaTime;
            if (_spawnPlateTimer > _spawnPlateTimerMax) {
                _spawnPlateTimer = 0f;
                if (_spawnedPlatesAmount < _spawnedPlatesMaxAmount) {
                    _spawnedPlatesAmount++;
                    PlateSpawned?.Invoke();
                }
            }
        }

        public override void Interact(Player player) {
            if (player.HasKitchenObject() || _spawnedPlatesAmount == 0) return;

            _spawnedPlatesAmount--;
            KitchenObject.SpawnKitchenObject(plateKitchenObjectScriptable, player);
            PlateGrabbed?.Invoke();
        }
    }
}