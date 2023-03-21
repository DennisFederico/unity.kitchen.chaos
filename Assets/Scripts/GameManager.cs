using System;
using Player;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { private set; get; }

    public event EventHandler GameStateChanged;
    public event Action OnGamePaused;
    public event Action OnGameUnPaused;

    [SerializeField] private float roundDuration = 90f;

    private enum GameState {
        WaitingToStart,
        StartCountDown,
        GamePlaying,
        GamePaused,
        GameOver,
    }

    private GameState _gameState;
    // private float _waitingToStartTimer = 2f;
    private float _countDownTimer = 3f;
    private float _gamePlayTimeLeft;
    private bool _gamePaused;

    private void Awake() {
        Instance = this;
        _gameState = GameState.WaitingToStart;
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInputOnPauseAction;
        GameInput.Instance.OnInteractAction += GameInputOnInteractAction;
    }

    private void GameInputOnInteractAction() {
        if (_gameState == GameState.WaitingToStart) {
            _gameState = GameState.StartCountDown;
            GameStateChanged?.Invoke(this, EventArgs.Empty);
            GameInput.Instance.OnInteractAction -= GameInputOnInteractAction;
        }
    }

    private void GameInputOnPauseAction() {
        TogglePauseGame();
    }

    private void Update() {
        switch (_gameState) {
            case GameState.WaitingToStart:
                // _waitingToStartTimer -= Time.deltaTime;
                // if (_waitingToStartTimer < 0f) {
                //     _gameState = GameState.StartCountDown;
                //     GameStateChanged?.Invoke(this, EventArgs.Empty);
                // }
                break;
            case GameState.StartCountDown:
                _countDownTimer -= Time.deltaTime;
                if (_countDownTimer < 0f) {
                    _gameState = GameState.GamePlaying;
                    _gamePlayTimeLeft = roundDuration;
                    GameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GamePlaying:
                _gamePlayTimeLeft -= Time.deltaTime;
                if (_gamePlayTimeLeft < 0f) {
                    _gameState = GameState.GameOver;
                    GameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GamePaused:
                break;
            case GameState.GameOver:
                break;
        }
    }

    public void TogglePauseGame() {
        _gamePaused = !_gamePaused;
        if (_gamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
        } else {
            Time.timeScale = 1f;
            OnGameUnPaused?.Invoke();
        }
    }

    public bool IsGamePlaying() {
        return _gameState == GameState.GamePlaying;
    }

    public bool IsStartCountDownActive() {
        return _gameState == GameState.StartCountDown;
    }

    public float GetCountDownTimer() {
        return _countDownTimer;
    }

    public bool IsGameOver() {
        return _gameState == GameState.GameOver;
    }

    public float GetPlayTimeLeft() {
        return _gamePlayTimeLeft;
    }

    public float GetPlayTimeLeftNormalized() {
        return 1 - (_gamePlayTimeLeft / roundDuration);
    }
}