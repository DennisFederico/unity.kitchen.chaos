public class ClearCounter : BaseCounter {

    public override void Interact(Player player) {
        if (!HasKitchenObject() && player.HasKitchenObject()) {
            player.GetKitchenObject().SetKitchenObjectParent(this);
        } else if (HasKitchenObject() && !player.HasKitchenObject()) {
            GetKitchenObject().SetKitchenObjectParent(player);
        }
    }
}