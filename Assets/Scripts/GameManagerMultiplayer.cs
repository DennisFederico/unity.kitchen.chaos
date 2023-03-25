using System;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerMultiplayer : NetworkBehaviour {
    
    public static GameManagerMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListScriptable kitchenObjectListScriptable;

    private void Awake() {
        Instance = this;
    }
    
    public void SpawnKitchenObject(KitchenObjectScriptable kitchenObjectScriptable, IKitchenObjectParent kitchenObjectParent) {
        var indexOf = kitchenObjectListScriptable.kitchenObjects.IndexOf(kitchenObjectScriptable);
        SpawnKitchenObjectServerRpc(indexOf, kitchenObjectParent.GetNetworkObject());
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectScriptableIndex, NetworkObjectReference kitchenObjectParentNetworkReference) {
        var kitchenObjectScriptable = kitchenObjectListScriptable.kitchenObjects[kitchenObjectScriptableIndex];
        var kitchenObjectInstance = Instantiate(kitchenObjectScriptable.prefab);
        kitchenObjectInstance.GetComponent<NetworkObject>().Spawn(true);
        
        kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        var kitchenObject = kitchenObjectInstance.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }
}