using KitchenObjects;

namespace Counters {
    public class ClearCounter : BaseCounter {

        public override void Interact(Player.Player player) {
            if (!HasKitchenObject() && player.HasKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else if (HasKitchenObject()) { 
                if (!player.HasKitchenObject()) { 
                    GetKitchenObject().SetKitchenObjectParent(player);
                } else if (player.GetKitchenObject().TryGetAsPlate(out var playerPlate)) {
                    //If it is a plate, add the ingredient to the plate
                    if (playerPlate.TryAddIngredient(GetKitchenObject().KitchenObjectScriptable)) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //GetKitchenObject().DestroySelf();
                    }
                } else if (GetKitchenObject().TryGetAsPlate(out var counterPlate)) {
                    if (counterPlate.TryAddIngredient(player.GetKitchenObject().KitchenObjectScriptable)) {
                        KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        //player.GetKitchenObject().DestroySelf();
                    }
                }
            }
        }
    }
}