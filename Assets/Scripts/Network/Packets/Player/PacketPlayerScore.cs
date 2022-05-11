using System;

namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerScore : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerScore;

        public Guid Id { get; set; }
        public PlayerScore Score { get; set; }
    }
}