using System;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Network.Packets.Game
{
    public class PacketBlockBulkMove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.BlockBulkMove;
        
        public ulong ContainerId { get; set; }
        public (Guid, Vector3Int)[] Positions { get; set; }
    }
}