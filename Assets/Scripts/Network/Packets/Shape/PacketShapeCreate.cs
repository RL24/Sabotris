using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Game
{
    public class PacketShapeCreate : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ShapeCreate;
        
        public ulong ContainerId { get; set; }
        public Guid Id { get; set; }
        public Vector3Int Position { get; set; }
        public (Guid, Vector3Int)[] Offsets { get; set; }
        public Color Color { get; set; }
    }
}