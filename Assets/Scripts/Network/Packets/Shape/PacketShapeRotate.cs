using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Shape
{
    public class PacketShapeRotate : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ShapeRotate;

        public Guid Id { get; set; }
        public Quaternion Rotation { get; set; }
    }
}