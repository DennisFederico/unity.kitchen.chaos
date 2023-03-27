using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour {
    
    public static CharacterSelectReady Instance { get; private set; }
    
    private Dictionary<ulong, bool> _playersReady;

    private void Awake() {
        Instance = this;
        _playersReady = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady() {
        SetPlayerReadyServerRpc();
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
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
}