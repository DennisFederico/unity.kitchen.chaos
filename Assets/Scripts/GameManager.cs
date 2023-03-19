using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { private set; get; }

    public event EventHandler GameStateChanged;

    [SerializeField] private float roundDuration = 90f;

    private enum GameState {
        WaitingToStart,
        StartCountDown,
        GamePlaying,
        GamePaused,
        GameOver,
    }

    private GameState _gameState;
    private float _waitingToStartTimer = 2f;
    private float _countDownTimer = 3f;
    private float _gamePlayTimeLeft;

    private void Awake() {
        Instance = this;
        _gameState = GameState.WaitingToStart;
    }

    private void Update() {
        switch (_gameState) {
            case GameState.WaitingToStart:
                _waitingToStartTimer -= Time.deltaTime;
                if (_waitingToStartTimer < 0f) {
                    _gameState = GameState.StartCountDown;
                    GameStateChanged?.Invoke(this, EventArgs.Empty);
                }

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