using System;

namespace Sabotris.Network.Packets.Game
{
    public class PacketBlockBulkRemove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.BlockBulkRemove;

        public ulong ContainerId { get; set; }
        public Guid[] Ids { get; set; }
    }
}