using KitchenObjects;

namespace Counters {
    public class ClearCounter : BaseCounter {

        public override void Interact(Player.Player player) {
            if (!HasKitchenObject() && player.HasKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else if (HasKitchenObject()) { 
                if (!player.HasKitchenObject()) { 
                    GetKitchenObject().SetKitchenObjectParent(player);
                } else if (player.GetKitchenObject() is PlateKitchenObject) {
                    //If it is a plate, add the ingredient to the plate
                    var plateKitchenObject = player.GetKitchenObject() as PlateKitchenObject;
                    plateKitchenObject.AddIngredient(GetKitchenObject().KitchenObjectScriptable);
                    GetKitchenObject().DestroySelf();
                }
            }
        }
    }
}