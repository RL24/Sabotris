using System;
using Sabotris.Util;

namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerDead : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerDead;
        
        public ulong Id { get; set; }
        public (Guid, int)[] BlockIndices { get; set; }
    }
}