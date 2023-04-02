using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameLobby : MonoBehaviour {
    public static GameLobby Instance { get; private set; }

    public event EventHandler CreateLobbyStarted;
    public event EventHandler CreateLobbyFailed;
    public event EventHandler JoinLobbyStarted;
    public event EventHandler JoinLobbyFailed;
    public event EventHandler QuickJoinLobbyFailed;
    public event Action<List<Lobby>> LobbyListChanged;

    private Lobby _joinedLobby;
    private const float HeartbeatTimerMax = 15f;
    private float _lobbyHeartbeatTimer;
    private const float LobbyListRefreshTimerMax = 3f;
    private float _lobbyListRefreshTimer;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private void Update() {
        HandleLobbyHeartbeat();
        HandleLobbyListRefresh();
    }

    private void HandleLobbyHeartbeat() {
        if (IsLobbyHost()) {
            _lobbyHeartbeatTimer -= Time.unscaledDeltaTime;
            if (_lobbyHeartbeatTimer <= 0f) {
                _lobbyHeartbeatTimer = HeartbeatTimerMax;
                LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }
    }

    private void HandleLobbyListRefresh() {
        if (_joinedLobby == null &&
            AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()
           ) {
            _lobbyListRefreshTimer -= Time.unscaledDeltaTime;
            if (_lobbyListRefreshTimer <= 0f) {
                _lobbyListRefreshTimer = LobbyListRefreshTimerMax;
                RefreshLobbiesList();
            }
        }
    }

    private bool IsLobbyHost() {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }


    private async void InitializeUnityAuthentication() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            InitializationOptions initializationOptions = new();
            #if LOCAL_BUILD            
            initializationOptions.SetProfile($"Player_{Random.Range(10000, 99999)}");
            #endif
            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    #region Relay

    private const string RelayJoinCodeLobbyDataKey = "RelayJoinCode";
    private async Task<Allocation> AllocateRelay() {
        try {
            var allocation = await RelayService.Instance.CreateAllocationAsync(GameManagerMultiplayer.MaxPlayers - 1);
            return allocation;
        } catch (RelayServiceException e) {
            Debug.LogError(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation) {
        try {
            var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        } catch (RelayServiceException e) {
            Debug.LogError(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string relayCode) {
        try {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
            return joinAllocation;
        } catch (RelayServiceException e) {
            Debug.LogError(e);
            return default;
        }
    }

    private async Task SetupNetworkTransportRelayServerJoinCode() {
        string relayJoinCode = _joinedLobby.Data[RelayJoinCodeLobbyDataKey].Value;
        var joinAllocation = await JoinRelay(relayJoinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
    }
    
    #endregion
    
    public async void CreateLobby(string lobbyName, bool isPrivate = false) {
        CreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try {
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameManagerMultiplayer.MaxPlayers, new CreateLobbyOptions() {
                IsPrivate = isPrivate
            });

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);
            //Send relay code as part of the lobby Data
            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions() {
                Data = new Dictionary<string, DataObject> {
                    { RelayJoinCodeLobbyDataKey,new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            
            GameManagerMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectionScene);
        } catch (LobbyServiceException e) {
            Debug.LogError(e);
            CreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    private async void RefreshLobbiesList() {
        try {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions() {
                Filters = new List<QueryFilter>() {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            LobbyListChanged?.Invoke(queryResponse.Results);
        } catch (LobbyServiceException e) {
            Debug.LogError(e);
        }
    }

    public async void QuickJoin() {
        JoinLobbyStarted?.Invoke(this, EventArgs.Empty);
        try {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            await SetupNetworkTransportRelayServerJoinCode();
            GameManagerMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.LogError(e);
            QuickJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithId(string lobbyId) {
        JoinLobbyStarted?.Invoke(this, EventArgs.Empty);
        try {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            await SetupNetworkTransportRelayServerJoinCode();
            GameManagerMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e) {
            JoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.LogError(e);
        }
    }

    public async void JoinWithCode(string lobbyCode) {
        JoinLobbyStarted?.Invoke(this, EventArgs.Empty);
        try {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            await SetupNetworkTransportRelayServerJoinCode();
            GameManagerMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e) {
            JoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.LogError(e);
        }
    }

    public void DeleteLobby() {
        if (_joinedLobby != null) {
            try {
                LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                _joinedLobby = null;
            } catch (LobbyServiceException e) {
                Debug.LogError(e);
            }
        }
    }

    public void LeaveLobby() {
        if (_joinedLobby != null) {
            try {
                LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                _joinedLobby = null;
            } catch (LobbyServiceException e) {
                Debug.LogError(e);
            }
        }
    }

    public void KickPlayerFromLobby(string playerId) {
        if (IsLobbyHost()) {
            try {
                LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
                _joinedLobby = null;
            } catch (LobbyServiceException e) {
                Debug.LogError(e);
            }
        }
    }

    public Lobby GetJoinedLobby() {
        return _joinedLobby;
    }
}