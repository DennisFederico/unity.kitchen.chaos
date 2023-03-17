namespace Counters {
    public class DeliveryCounter : BaseCounter {
        public override void Interact(Player.Player player) {
            if (!player.HasKitchenObject() ) return;
            if (player.GetKitchenObject().TryGetAsPlate(out var plateKitchenObject)) {
                plateKitchenObject.DestroySelf();
            }
        }
    }
}
