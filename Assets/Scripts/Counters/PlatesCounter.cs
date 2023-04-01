using System;
using KitchenObjects;
using KitchenPlayer;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counters {
    public class PlatesCounter : BaseCounter {

        public event Action PlateSpawned;
        public event Action PlateGrabbed;
        [SerializeField] private KitchenObjectScriptable plateKitchenObjectScriptable;
        private float _spawnPlateTimer;
        private readonly float _spawnPlateTimerMax = 4f;
        private int _spawnedPlatesAmount;
        private readonly int _spawnedPlatesMaxAmount = 4;

        private void Update() {
            if (!IsServer) return;

            if (!GameManager.Instance.IsGamePlaying()) return;
            _spawnPlateTimer += Time.deltaTime;
            if (_spawnPlateTimer > _spawnPlateTimerMax) {
                _spawnPlateTimer = 0f;
                if (_spawnedPlatesAmount < _spawnedPlatesMaxAmount) {
                    SpawnPlateServerRpc();
                }
            }
        }

        [ServerRpc]
        private void SpawnPlateServerRpc() {
            SpawnPlateClientRpc();
        }

        [ClientRpc]
        private void SpawnPlateClientRpc() {
            _spawnedPlatesAmount++;
            PlateSpawned?.Invoke();
        }

        public override void Interact(Player player) {
            if (player.HasKitchenObject() || _spawnedPlatesAmount == 0) return;
            KitchenObject.SpawnKitchenObject(plateKitchenObjectScriptable, player);
            InteractLogicServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc() {
            InteractLogicClientRpc();
        }

        [ClientRpc]
        private void InteractLogicClientRpc() {
            _spawnedPlatesAmount--;
            PlateGrabbed?.Invoke();
        }
    }
}