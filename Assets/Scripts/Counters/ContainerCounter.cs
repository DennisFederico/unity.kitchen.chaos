using System;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counters {
    public class ContainerCounter : BaseCounter {

        public event Action OnGrabObjectFromContainer;
        [SerializeField] private KitchenObjectScriptable kitchenObjectScriptable;

        public Sprite GetContainerSprite() {
            return kitchenObjectScriptable.sprite;
        }
        
        public override void Interact(Player.Player player) {
            if (!player.HasKitchenObject()) {
                KitchenObject.SpawnKitchenObject(kitchenObjectScriptable, player);
                InteractLogicServerRpc();
            } else if (player.GetKitchenObject().TryGetAsPlate(out var plateKitchenObject)) {
                if (plateKitchenObject.TryAddIngredient(kitchenObjectScriptable)) {
                    InteractLogicServerRpc();
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc() {
            InteractLogicClientRpc();
        }

        [ClientRpc]
        private void InteractLogicClientRpc() {
            OnGrabObjectFromContainer?.Invoke();
        }
        
    }
}