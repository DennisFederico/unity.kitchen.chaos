using TMPro;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour {

    [SerializeField] private int playerIndex;
    [SerializeField] private TMP_Text readyText;
    private void Start() {
        GameManagerMultiplayer.Instance.PlayerDataNetworkListChanged += GameManagerMultiplayerOnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnAnyPlayerReady += CharacterSelectReadyOnAnyPlayerReady;
        UpdatePlayer();
    }

    private void CharacterSelectReadyOnAnyPlayerReady() {
        UpdatePlayer();
    }

    private void GameManagerMultiplayerOnPlayerDataNetworkListChanged() {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (GameManagerMultiplayer.Instance.TryGetPlayerDataForPlayerIndex(playerIndex, out var playerData)) {
            readyText.gameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
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