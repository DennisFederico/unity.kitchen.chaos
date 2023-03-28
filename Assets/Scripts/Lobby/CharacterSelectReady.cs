using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Lobby {
    public class CharacterSelectReady : NetworkBehaviour {
    
        public static CharacterSelectReady Instance { get; private set; }
        public event Action OnAnyPlayerReady; 
        private Dictionary<ulong, bool> _playersReady;

        private void Awake() {
            Instance = this;
            _playersReady = new Dictionary<ulong, bool>();
        }

        public void SetPlayerReady() {
            SetPlayerReadyServerRpc();
        }

        public bool IsPlayerReady(ulong clientId) {
            return _playersReady.ContainsKey(clientId) && _playersReady[clientId];
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
            SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
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

        [ClientRpc]
        private void SetPlayerReadyClientRpc(ulong clientId) {
            _playersReady[clientId] = true;
            OnAnyPlayerReady?.Invoke();
        }
    }
}