using System;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Network.Packets.Game
{
    public class PacketBlockBulkRemove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.BlockBulkRemove;
        
        public long ContainerId { get; set; }
        public Guid[] Ids { get; set; }
    }
}