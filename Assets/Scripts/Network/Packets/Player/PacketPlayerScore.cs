using System;
using Sabotris.Game;

namespace Sabotris.Network.Packets.Players
{
    public class PacketPlayerScore : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerScore;

        public Guid Id { get; set; }
        public PlayerScore Score { get; set; }
    }
}