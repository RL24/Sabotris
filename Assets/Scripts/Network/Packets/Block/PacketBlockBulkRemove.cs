using System;

namespace Sabotris.Network.Packets.Block
{
    public class PacketBlockBulkRemove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.BlockBulkRemove;

        public Guid ContainerId { get; set; }
        public Guid[] Ids { get; set; }
    }
}