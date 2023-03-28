using System;
using Unity.Netcode;

namespace Lobby {
    public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
        public ulong clientId;
        public int colorId;
        public byte position;

        public bool Equals(PlayerData other) {
            return clientId == other.clientId && colorId == other.colorId && position == other.position;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref colorId);
            serializer.SerializeValue(ref position);
        }
    }
}