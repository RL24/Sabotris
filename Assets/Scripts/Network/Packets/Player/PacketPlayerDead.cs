using System;

namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerDead : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerDead;

        public Guid Id { get; set; }
    }
}