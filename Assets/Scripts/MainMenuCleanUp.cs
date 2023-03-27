using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour {
    private void Awake() {
        if (NetworkManager.Singleton) {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (GameManagerMultiplayer.Instance) {
            Destroy(GameManagerMultiplayer.Instance.gameObject);
        }
    }
}