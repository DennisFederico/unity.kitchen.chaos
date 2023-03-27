using KitchenObjects;

namespace Counters {
    public class DeliveryCounter : BaseCounter {
        public override void Interact(Player.Player player) {
            if (!player.HasKitchenObject() ) return;
            if (!player.GetKitchenObject().TryGetAsPlate(out var plateKitchenObject)) return;
            DeliveryManager.Instance.DeliverPlate(this, plateKitchenObject);
            KitchenObject.DestroyKitchenObject(plateKitchenObject);
        }
    }
}
