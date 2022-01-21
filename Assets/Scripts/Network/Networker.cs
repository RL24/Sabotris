using System.Collections.Generic;
using JetBrains.Annotations;
using Lidgren.Network;
using Sabotris.Network.Packets;

namespace Sabotris.Network
{
    public static class Reasons
    {
        public const string NoReason = "";
        public const string RestartServer = "Restarting server";
        public const string RestartClient = "Restarting client";
        public const string InvalidConnectPacket = "Invalid connect payload";
        public const string IncorrectPassword = "Incorrect password";
        public const string NoPlayersLeft = "No remaining players";
    }
    
    public abstract class Networker<T> where T : NetPeer
    {
        protected T Peer;
        protected readonly Dictionary<long, Player> Players = new Dictionary<long, Player>();
        protected PacketHandler PacketHandler { get; }
        protected bool Running { get; set; }

        protected Networker(PacketDirection packetDirection)
        {
            PacketHandler = new PacketHandler(packetDirection);
        }

        public void RegisterListener<TU>(TU instance)
        {
            PacketHandler.Register(instance);
        }

        public void DeregisterListener<TU>(TU instance)
        {
            PacketHandler.Deregister(instance);
        }

        public virtual void Shutdown(string reason)
        {
            Peer.FlushSendQueue();
            Peer.Shutdown(reason);
            Players.Clear();
            Running = false;
        }
        
        protected Packet GetPacket(NetIncomingMessage incoming)
        {
            var packetType = PacketTypes.GetPacketType((PacketTypeId) incoming.ReadByte());
            var packet = packetType.NewPacket();
            packet.Deserialize(incoming);
            return packet;
        }
    }
}