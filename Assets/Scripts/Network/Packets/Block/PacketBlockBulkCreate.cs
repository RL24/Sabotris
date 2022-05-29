using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Block
{
    public class PacketBlockBulkCreate : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.BlockBulkCreate;

        public Guid ContainerId { get; set; }
        public (Guid, Vector3Int, Color)[] Blocks { get; set; }
    }
}