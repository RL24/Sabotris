using System;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Network.Packets.Game
{
    public class PacketShapeLock : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ShapeLock;
        
        public Guid Id { get; set; }
        public Vector3Int[] Offsets { get; set; }
    }
}