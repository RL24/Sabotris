using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Game
{
    public class PacketShapeMove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ShapeMove;
        
        public Guid Id { get; set; }
        public Vector3Int Position { get; set; }
    }
}