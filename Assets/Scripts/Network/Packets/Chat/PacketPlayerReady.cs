using System;

namespace Sabotris.Network.Packets.Chat
{
    public class PacketPlayerReady : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerReady;

        public Guid Id { get; set; }
        public bool Ready { get; set; }
    }
}