using System;
using KitchenObjects;

namespace Counters {
    public class TrashCounter : BaseCounter {

        public static event EventHandler ObjectTrashed;
        public new static void ResetStaticEventHandler() {
            ObjectTrashed = null;
        }
        public override void Interact(Player.Player player) {
            if (!player.HasKitchenObject()) return;
            TrashKitchenObject(player.GetKitchenObject());
        }

        public override void InteractAlternate(Player.Player player) {
            if (!player.HasKitchenObject()) return;
            TrashKitchenObject(player.GetKitchenObject());
        }

        private void TrashKitchenObject(KitchenObject kitchenObject) {
            kitchenObject.DestroySelf();
            ObjectTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}