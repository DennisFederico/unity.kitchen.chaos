using System;
using UnityEngine;

public class ContainerCounter : BaseCounter {

    public event Action OnGrabObjectFromContainer;
    [SerializeField] private KitchenObjectScriptable kitchenObjectScriptable;

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) return;
        KitchenObject.SpawnKitchenObject(kitchenObjectScriptable, player);
        OnGrabObjectFromContainer?.Invoke();
    }

    public Sprite GetContainerSprite() {
        return kitchenObjectScriptable.sprite;
    }
}