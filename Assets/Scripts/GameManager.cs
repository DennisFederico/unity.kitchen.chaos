using System;
using System.Collections.Generic;
using KitchenPlayer;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
    public static GameManager Instance { private set; get; }

    public event EventHandler GameStateChanged;
    public event EventHandler LocalPlayerReadyChanged;
    public event Action OnLocalGamePaused;
    public event Action OnLocalGameUnPaused;
    public event Action OnMultiplayerGamePaused;
    public event Action OnMultiplayerGameUnPaused;

    [SerializeField] private Transform playerPrefab;
    [SerializeField] private float roundDuration = 90f;
    [SerializeField] private float countDown = 3f;
    [SerializeField] private GameState startState = GameState.WaitingToStart;

    private enum GameState {
        WaitingToStart,
        StartCountDown,
        GamePlaying,
        GamePaused,
        GameOver,
    }

    private readonly NetworkVariable<GameState> _gameState = new();
    private bool _isLocalPlayerReady;
    private readonly NetworkVariable<float> _countDownTimer = new();
    private readonly NetworkVariable<float> _gamePlayTimeLeft = new();
    private bool _isLocalGamePaused;
    private readonly NetworkVariable<bool> _isGamePaused = new NetworkVariable<bool>();
    private Dictionary<ulong, bool> _playersReady;
    private Dictionary<ulong, bool> _playersPaused;
    private bool _playerDisconnected;

    private void Awake() {
        Instance = this;
        _playersReady = new Dictionary<ulong, bool>();
        _playersPaused = new Dictionary<ulong, bool>();
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInputOnPauseAction;
        GameInput.Instance.OnInteractAction += GameInputOnInteractAction;
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            _gameState.Value = startState;
            _countDownTimer.Value = countDown;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
        }
        _gameState.OnValueChanged += (_, _) => GameStateChanged?.Invoke(this, EventArgs.Empty);
        _isGamePaused.OnValueChanged += (_, _) => OnGamePauseValueChanged();
    }

    private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (var clientId in clientsCompleted) {
            var player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManagerOnClientDisconnectCallback(ulong clientId) {
        _playerDisconnected = true;
    }

    private void OnGamePauseValueChanged() {
        //DONT CHANGE TIMESCALE IF YOU DONT WANT TO PAUSE THE GAME
        if (_isGamePaused.Value) {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke();
        } else {
            Time.timeScale = 1f;
            OnMultiplayerGameUnPaused?.Invoke();
        }
    }

    private void GameInputOnInteractAction() {
        if (_gameState.Value == GameState.WaitingToStart) {
            _isLocalPlayerReady = true;
            LocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    private void GameInputOnPauseAction() {
        TogglePauseGame();
    }

    private void Update() {
        if (!IsServer) return;
        switch (_gameState.Value) {
            default:
            case GameState.WaitingToStart:
                break;
            case GameState.StartCountDown:
                _countDownTimer.Value -= Time.deltaTime;
                if (_countDownTimer.Value < 0f) {
                    _gameState.Value = GameState.GamePlaying;
                    _gamePlayTimeLeft.Value = roundDuration;
                }

                break;
            case GameState.GamePlaying:
                _gamePlayTimeLeft.Value -= Time.deltaTime;
                if (_gamePlayTimeLeft.Value < 0f) {
                    _gameState.Value = GameState.GameOver;
                }

                break;
            case GameState.GamePaused:
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void LateUpdate() {
        if (!IsServer) return;
        if (_playerDisconnected) {
            _playerDisconnected = false;
            _isGamePaused.Value = TestGamePausedState();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        _playersReady[serverRpcParams.Receive.SenderClientId] = true;
    
        bool allPlayersReady = true;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!_playersReady.ContainsKey(clientId) || !_playersReady[clientId]) {
                allPlayersReady = false;
                break;
            }
        }
    
        if (allPlayersReady) {
            _gameState.Value = GameState.StartCountDown;
        }
    }

    public void TogglePauseGame() {
        _isLocalGamePaused = !_isLocalGamePaused;
        if (_isLocalGamePaused) {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke();
        } else {
            UnPauseGameServerRpc();
            OnLocalGameUnPaused?.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        _playersPaused[serverRpcParams.Receive.SenderClientId] = true;
        _isGamePaused.Value = TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        _playersPaused[serverRpcParams.Receive.SenderClientId] = false;
        _isGamePaused.Value = TestGamePausedState();
    }

    private bool TestGamePausedState() {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (_playersPaused.ContainsKey(clientId) && _playersPaused[clientId]) {
                return true;
            }
        }
        return false;
    }

    public bool IsWaitingToStart() {
        return _gameState.Value == GameState.WaitingToStart;
    }

    public bool IsGamePlaying() {
        return _gameState.Value == GameState.GamePlaying;
    }

    public bool IsStartCountDownActive() {
        return _gameState.Value == GameState.StartCountDown;
    }

    public float GetCountDownTimer() {
        return _countDownTimer.Value;
    }

    public bool IsLocalPlayerReady() {
        return _isLocalPlayerReady;
    }

    public bool IsGameOver() {
        return _gameState.Value == GameState.GameOver;
    }

    public float GetPlayTimeLeft() {
        return _gamePlayTimeLeft.Value;
    }

    public float GetPlayTimeLeftNormalized() {
        return 1 - (_gamePlayTimeLeft.Value / roundDuration);
    }
}