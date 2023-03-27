using System;
using KitchenObjects;
using Lobby;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMultiplayer : NetworkBehaviour {
    
    public static GameManagerMultiplayer Instance { get; private set; }
    [SerializeField] private KitchenObjectListScriptable kitchenObjectListScriptable;

    private const string GameFullConnectionErrorMessage = "Game is full";
    private const string GameStartedConnectionErrorMessage = "Game already started!";
    private const int MaxPlayers = 4;
    private NetworkList<PlayerData> _playerDataNetworkList;
    public event Action TryingToJoinGame;
    public event Action FailedToJoinGame;
    public event Action PlayerDataNetworkListChanged;
    
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //NetworkList cannot be initialized on declaration or OnSpawn
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkListOnListChanged;
    }

    private void PlayerDataNetworkListOnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        PlayerDataNetworkListChanged?.Invoke();
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerOnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManagerOnClientConnectedCallback(ulong clientId) {
        _playerDataNetworkList.Add(new PlayerData() {
            clientId = clientId
        });
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectionScene.ToString()) {
            response.Approved = false;
            response.Reason = GameStartedConnectionErrorMessage;
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MaxPlayers) {
            response.Approved = false;
            response.Reason = GameFullConnectionErrorMessage;
            return;
        }
        response.Approved = true;
    }

    public void StartClient() {
        TryingToJoinGame?.Invoke();
        NetworkManager.Singleton.OnClientDisconnectCallback += _ => FailedToJoinGame?.Invoke(); 
        NetworkManager.Singleton.StartClient();
    }
    
    public int GetKitchenObjectScriptableIndex(KitchenObjectScriptable kitchenObjectScriptable) {
        var kitchenObjectScriptableIndex = kitchenObjectListScriptable.kitchenObjects.IndexOf(kitchenObjectScriptable);
        return kitchenObjectScriptableIndex;
    }

    public KitchenObjectScriptable GetKitchenObjectScriptable(int index) {
        return kitchenObjectListScriptable.kitchenObjects[index];
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

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkReference) {
        kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        ClearKitchenObjectFromParentClientRpc(kitchenObjectNetworkReference);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectFromParentClientRpc(NetworkObjectReference kitchenObjectNetworkReference) {
        kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectFromParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < _playerDataNetworkList.Count;
    }
    
    public bool TryGetPlayerDataForPlayerIndex(int playerIndex, out PlayerData playerData) {
        if (IsPlayerIndexConnected(playerIndex)) {
            playerData = _playerDataNetworkList[playerIndex];
            return true;
        }
        playerData = default;
        return false;
    }
}