using System.Runtime.InteropServices;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Steamworks;

namespace Network
{
    public enum DisconnectReason
    {
        None,
        ConnectionClosed,
        LobbyHostIdNotFound,
        ServerClosed,
        ConnectionIssue,
        ClientDisconnected,
        ClientLeftLobby
    }
    
    public class Networker
    {
        public const string HostIdKey = "HostId";
        public const string LobbyNameKey = "LobbyName";
        
        protected readonly NetworkController NetworkController;
        public readonly PacketHandler PacketHandler;
        
        protected Networker(NetworkController networkController, PacketDirection packetDirection)
        {
            NetworkController = networkController;
            PacketHandler = new PacketHandler(packetDirection);
        }

        public void RegisterListener<T>(T instance)
        {
            PacketHandler.Register(instance);
        }

        public void DeregisterListener<T>(T instance)
        {
            PacketHandler.Deregister(instance);
        }
        
        protected static Packet GetPacket(SteamNetworkingMessage_t message)
        {
            var data = ParseMessageData(message);
            var incoming = new ByteBuffer(data);
            
            var senderId = incoming.ReadUInt64();
            var packetType = PacketTypes.GetPacketType((PacketTypeId) incoming.ReadByte());
            var packet = packetType.NewPacket();
            packet.Connection = message.m_conn;
            packet.SenderId = senderId;
            packet.Deserialize(incoming);
            return packet;
        }

        private static byte[] ParseMessageData(SteamNetworkingMessage_t message)
        {
            var bytes = new byte[message.m_cbSize];
            Marshal.Copy(message.m_pData, bytes, 0, message.m_cbSize);
            return bytes;
        }
    }
}