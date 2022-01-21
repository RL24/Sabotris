using System;
using Sabotris.Util;

namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerDead : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerDead;
        
        public long Id { get; set; }
        public Pair<Guid, int>[] BlockIndices { get; set; }
    }
}