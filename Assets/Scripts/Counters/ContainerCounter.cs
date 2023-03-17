using System;
using KitchenObjects;
using ScriptableObjects;
using UnityEngine;

namespace Counters {
    public class ContainerCounter : BaseCounter {

        public event Action OnGrabObjectFromContainer;
        [SerializeField] private KitchenObjectScriptable kitchenObjectScriptable;

        public override void Interact(Player.Player player) {
            if (!player.HasKitchenObject()) {
                KitchenObject.SpawnKitchenObject(kitchenObjectScriptable, player);
                OnGrabObjectFromContainer?.Invoke();
            } else if (player.GetKitchenObject().TryGetAsPlate(out var plateKitchenObject)) {
                if (plateKitchenObject.TryAddIngredient(kitchenObjectScriptable)) {
                    OnGrabObjectFromContainer?.Invoke();
                }
            }
        }

        public Sprite GetContainerSprite() {
            return kitchenObjectScriptable.sprite;
        }
    }
}