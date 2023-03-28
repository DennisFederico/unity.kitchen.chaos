using System;
using System.Collections.Generic;
using KitchenObjects;
using Lobby;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManagerMultiplayer : NetworkBehaviour {
    
    public static GameManagerMultiplayer Instance { get; private set; }
    [SerializeField] private KitchenObjectListScriptable kitchenObjectListScriptable;
    [SerializeField] private List<Color> playerColorList;

    private const string GameFullConnectionErrorMessage = "Game is full";
    private const string GameStartedConnectionErrorMessage = "Game already started!";
    private const int MaxPlayers = 4;
    private NetworkList<PlayerData> _playerDataNetworkList;
    public event Action TryingToJoinGame;
    public event Action FailedToJoinGame;
    public event Action AnyPlayerDataChanged;
    public event Action<int, int> LocalPlayerColorChanged;
    
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //NetworkList cannot be initialized on declaration or OnSpawn
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkListOnListChanged;
    }

    private void PlayerDataNetworkListOnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        if (changeEvent.Type != NetworkListEvent<PlayerData>.EventType.RemoveAt && changeEvent.Value.clientId == NetworkManager.Singleton.LocalClientId) {
            LocalPlayerColorChanged?.Invoke(changeEvent.PreviousValue.colorId, changeEvent.Value.colorId);    
        }
        AnyPlayerDataChanged?.Invoke();
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerHostOnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManagerHostOnClientDisconnectCallback(ulong clientId) {
        if (TryGetPlayerDataIndexForClientId(clientId, out var playerIndex)) {
            _playerDataNetworkList.RemoveAt(playerIndex);
        }
    }

    private void NetworkManagerOnClientConnectedCallback(ulong clientId) {
        _playerDataNetworkList.Add(new PlayerData() {
            clientId = clientId,
            colorId = GetFirstRandomUnusedColorId()
        });
    }

    private int GetFirstRandomUnusedColorId() {
        //RandomStart
        var colorId = Random.Range(0, playerColorList.Count);
        while (true) {
            if (IsColorAvailable(colorId)) return colorId;
            colorId = (colorId + 1) % playerColorList.Count;
        }
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

    private bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < _playerDataNetworkList.Count;
    }

    public void KickPlayer(ulong clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManagerHostOnClientDisconnectCallback(clientId);
    }
    
    public bool TryGetPlayerDataForPlayerIndex(int playerIndex, out PlayerData playerData) {
        if (IsPlayerIndexConnected(playerIndex)) {
            playerData = _playerDataNetworkList[playerIndex];
            return true;
        }
        playerData = default;
        return false;
    }
    
    public bool TryGetPlayerDataForClientId(ulong clientId, out PlayerData playerData) {
        foreach (var player in _playerDataNetworkList) {
            if (player.clientId == clientId) {
                playerData = player;
                return true;
            }
        }
        playerData = default;
        return false;
    }

    public int GetPlayerDataIndexForClientId(ulong clientId) {
        for (int index = 0; index < _playerDataNetworkList.Count; index++) {
            if (_playerDataNetworkList[index].clientId == clientId) {
                return index;
            }
        }
        return -1;
    }
    
    private bool TryGetPlayerDataIndexForClientId(ulong clientId, out int playerDataIndex) {
        playerDataIndex = GetPlayerDataIndexForClientId(clientId);
        return playerDataIndex != -1;
    }

    public List<Color> GetPlayerColorList() {
        return playerColorList;
    }
    
    public Color GetPlayerColorByColorId(int colorIndex) {
        return playerColorList[colorIndex];
    }

    public void ChangePlayerColor(int colorId) {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc (RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
        if (!IsColorAvailable(colorId)) return;
        if (!TryGetPlayerDataIndexForClientId(serverRpcParams.Receive.SenderClientId, out var playerDataIndex)) return;
        var playerData = _playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId) {
        foreach (var playerData in _playerDataNetworkList) {
            if (playerData.colorId == colorId) {
                return false;
            }
        }
        return true;
    }
}