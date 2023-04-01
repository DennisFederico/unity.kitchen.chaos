using System;
using KitchenObjects;
using KitchenPlayer;
using Unity.Netcode;

namespace Counters {
    public class TrashCounter : BaseCounter {

        public static event EventHandler ObjectTrashed;
        public new static void ResetStaticEventHandler() {
            ObjectTrashed = null;
        }
        public override void Interact(Player player) {
            if (!player.HasKitchenObject()) return;
            TrashKitchenObject(player.GetKitchenObject());
        }

        public override void InteractAlternate(Player player) {
            if (!player.HasKitchenObject()) return;
            TrashKitchenObject(player.GetKitchenObject());
        }

        private void TrashKitchenObject(KitchenObject kitchenObject) {
            KitchenObject.DestroyKitchenObject(kitchenObject);
            InteractLogicServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc() {
            InteractLogicClientRpc();
        }

        [ClientRpc]
        private void InteractLogicClientRpc() {
            ObjectTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}