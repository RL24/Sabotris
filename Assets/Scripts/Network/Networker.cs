using System;
using System.Runtime.InteropServices;
using Sabotris.Network.Packets;
using Sabotris.Util;
using Steamworks;

namespace Sabotris.Network
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

    public enum SteamNetworkingSocketsSendType
    {
        Unreliable = 0,
        NoNagle = 1,
        NoDelay = 4,
        Reliable = 8
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

        protected void ProcessIncomingMessages(IntPtr[] receivedMessages, int incomingMessages)
        {
            if (incomingMessages == -1)
            {
                Logging.Log(true, "Polling messages failed, failed connection");
                return;
            }

            if (incomingMessages <= 0)
                return;

            if (incomingMessages > 20)
                Logging.Warn(false, "Received more than 20 messages, potentially lag");

            for (var i = 0; i < incomingMessages; i++)
            {
                var receivedMessage = receivedMessages[i];
                var parsedMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(receivedMessage);

                var packet = GetPacket(parsedMessage);

                SteamNetworkingMessage_t.Release(receivedMessage);

                PacketHandler.Process(packet);
            }
        }

        private static Packet GetPacket(SteamNetworkingMessage_t message)
        {
            var data = ParseMessageData(message);
            var incoming = new ByteBuffer(data);

            var senderId = incoming.ReadUInt64();
            var packetType = PacketTypes.GetPacketType((PacketTypeId) incoming.ReadByte());
            incoming.PacketType = packetType;

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

        protected void SendNetworkMessage(HSteamNetConnection connection, SteamNetworkingMessage_t buffer, uint length)
        {
            var res = SteamNetworkingSockets.SendMessageToConnection(connection, buffer.m_pData, length, (int) SteamNetworkingSocketsSendType.Reliable, out _);
            if (res != EResult.k_EResultOK)
                Logging.Log(true, "Failed to send packet to {0}: {1}", PacketHandler.PacketDirection == PacketDirection.Client ? "server" : "client", res);
        }
    }
}