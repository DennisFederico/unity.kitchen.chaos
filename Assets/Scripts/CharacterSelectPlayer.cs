using System;
using Player;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {

    [SerializeField] private int playerIndex;
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickPlayerButton;

    private void Awake() {
        kickPlayerButton.onClick.AddListener(() => {
            if (GameManagerMultiplayer.Instance.TryGetPlayerDataForPlayerIndex(playerIndex, out var playerData)) {
                GameManagerMultiplayer.Instance.KickPlayer(playerData.clientId);
            }
        });
    }

    private void Start() {
        GameManagerMultiplayer.Instance.AnyPlayerDataChanged += UpdatePlayer;
        CharacterSelectReady.Instance.OnAnyPlayerReady += UpdatePlayer;
        kickPlayerButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && playerIndex != 0);
        UpdatePlayer();
    }

    private void OnDestroy() {
        GameManagerMultiplayer.Instance.AnyPlayerDataChanged -= UpdatePlayer;
    }

    private void UpdatePlayer() {
        if (GameManagerMultiplayer.Instance.TryGetPlayerDataForPlayerIndex(playerIndex, out var playerData)) {
            readyText.gameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            playerVisual.SetPlayerColor(GameManagerMultiplayer.Instance.GetPlayerColorByColorId(playerData.colorId));
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
    
}