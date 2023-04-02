using System;
using System.Collections.Generic;
using KitchenLobby;
using KitchenObjects;
using ScriptableObjects;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManagerMultiplayer : NetworkBehaviour {
    public static GameManagerMultiplayer Instance { get; private set; }
    
    public const int MaxPlayers = 4;
    private const string PlayerPrefsPlayerName = "PlayerNameMultiplayer";
    
    [SerializeField] private KitchenObjectListScriptable kitchenObjectListScriptable;
    [SerializeField] private List<Color> playerColorList;

    private const string GameFullConnectionErrorMessage = "Game is full";
    private const string GameStartedConnectionErrorMessage = "Game already started!";
    
    private NetworkList<PlayerData> _playerDataNetworkList;
    private bool[] _playerPositions;
    private string _playerName;

    public string PlayerName {
        get => _playerName;
        set {
            _playerName = value;
            PlayerPrefs.SetString(PlayerPrefsPlayerName, _playerName);
        }
    }

    public event Action TryingToJoinGame;
    public event Action FailedToJoinGame;
    public event Action AnyPlayerDataChanged;
    public event Action<int, int> LocalPlayerColorChanged;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        PlayerName = PlayerPrefs.GetString(PlayerPrefsPlayerName, $"Player_{Random.Range(1000, 9999)}");
        
        //NetworkList cannot be initialized on declaration or OnSpawn
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerPositions = new bool[MaxPlayers];
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
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerHostOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerHostOnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient() {
        TryingToJoinGame?.Invoke();
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerClientOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += _ => FailedToJoinGame?.Invoke();
        NetworkManager.Singleton.StartClient();
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

    private void NetworkManagerClientOnClientConnectedCallback(ulong clientId) {
        SetPlayerNameServerRpc(_playerName);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
        if (!TryGetPlayerDataIndexForClientId(serverRpcParams.Receive.SenderClientId, out var playerDataIndex)) return;
        var playerData = _playerDataNetworkList[playerDataIndex];
        playerData.playerName = playerName;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) {
        if (!TryGetPlayerDataIndexForClientId(serverRpcParams.Receive.SenderClientId, out var playerDataIndex)) return;
        var playerData = _playerDataNetworkList[playerDataIndex];
        playerData.playerId = playerId;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }
    
    private void NetworkManagerHostOnClientDisconnectCallback(ulong clientId) {
        if (TryGetPlayerDataIndexForClientId(clientId, out var playerIndex)) {
            _playerPositions[_playerDataNetworkList[playerIndex].position] = false;
            _playerDataNetworkList.RemoveAt(playerIndex);
        }
    }

    private void NetworkManagerHostOnClientConnectedCallback(ulong clientId) {
        var playerPosition = GetFirstFreePlayerPosition();
        _playerDataNetworkList.Add(new PlayerData() {
            clientId = clientId,
            colorId = GetFirstRandomUnusedColorId(),
            position = playerPosition
        });
        _playerPositions[playerPosition] = true;
        SetPlayerNameServerRpc(_playerName);
    }
    
    public void KickPlayer(ulong clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManagerHostOnClientDisconnectCallback(clientId);
    }

    #region KitchenObjectScriptable
    public int GetKitchenObjectScriptableIndex(KitchenObjectScriptable kitchenObjectScriptable) {
        var kitchenObjectScriptableIndex = kitchenObjectListScriptable.kitchenObjects.IndexOf(kitchenObjectScriptable);
        return kitchenObjectScriptableIndex;
    }

    public KitchenObjectScriptable GetKitchenObjectScriptable(int index) {
        return kitchenObjectListScriptable.kitchenObjects[index];
    }
    #endregion

    #region KitchenObject
    public void SpawnKitchenObject(KitchenObjectScriptable kitchenObjectScriptable, IKitchenObjectParent kitchenObjectParent) {
        var indexOf = kitchenObjectListScriptable.kitchenObjects.IndexOf(kitchenObjectScriptable);
        SpawnKitchenObjectServerRpc(indexOf, kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectScriptableIndex, NetworkObjectReference kitchenObjectParentNetworkReference) {
        
        kitchenObjectParentNetworkReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        
        //Confirm the parent doesn't have an item already
        if (kitchenObjectParent.HasKitchenObject()) return;
        
        var kitchenObjectScriptable = kitchenObjectListScriptable.kitchenObjects[kitchenObjectScriptableIndex];
        var kitchenObjectInstance = Instantiate(kitchenObjectScriptable.prefab);
        kitchenObjectInstance.GetComponent<NetworkObject>().Spawn(true);
        
        var kitchenObject = kitchenObjectInstance.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkReference) {
        if (!kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject)) return;
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
    #endregion
    
    #region Handle PlayerData List
    private byte GetFirstFreePlayerPosition() {
        byte index = 0;
        for (; index < MaxPlayers; index++) {
            if (!_playerPositions[index]) break;
        }
        return index;
    }

    public bool TryGetPlayerDataForPlayerPosition(int playerPosition, out PlayerData playerData) {
        foreach (var player in _playerDataNetworkList) {
            if (player.position == playerPosition) {
                playerData = player;
                return true;
            }
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
    
    private bool TryGetPlayerDataIndexForClientId(ulong clientId, out int playerDataIndex) {
        playerDataIndex = GetPlayerDataIndexForClientId(clientId);
        return playerDataIndex != -1;
    }
    
    private int GetPlayerDataIndexForClientId(ulong clientId) {
        for (int index = 0; index < _playerDataNetworkList.Count; index++) {
            if (_playerDataNetworkList[index].clientId == clientId) {
                return index;
            }
        }

        return -1;
    }
    
    public int GetPlayerPositionForClientId(ulong clientId) {
        for (int index = 0; index < _playerDataNetworkList.Count; index++) {
            if (_playerDataNetworkList[index].clientId == clientId) {
                return _playerDataNetworkList[index].position;
            }
        }

        return -1;
    }
    
    #endregion


    #region PlayerColor
    
    public List<Color> GetPlayerColorList() {
        return playerColorList;
    }
    
    public Color GetPlayerColorByColorId(int colorIndex) {
        return playerColorList[colorIndex];
    }
    
    private int GetFirstRandomUnusedColorId() {
        //RandomStart
        var colorId = Random.Range(0, playerColorList.Count);
        while (true) {
            if (IsColorAvailable(colorId)) return colorId;
            colorId = (colorId + 1) % playerColorList.Count;
        }
    }
    
    private bool IsColorAvailable(int colorId) {
        foreach (var playerData in _playerDataNetworkList) {
            if (playerData.colorId == colorId) {
                return false;
            }
        }
        return true;
    }
    
    public void ChangePlayerColor(int colorId) {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
        if (!IsColorAvailable(colorId)) return;
        if (!TryGetPlayerDataIndexForClientId(serverRpcParams.Receive.SenderClientId, out var playerDataIndex)) return;
        var playerData = _playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }
    #endregion
    
}