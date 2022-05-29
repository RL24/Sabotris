using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Shape
{
    public class PacketShapeLock : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ShapeLock;

        public Guid Id { get; set; }
        public Vector3Int LockPos { get; set; }
        public Quaternion LockRot { get; set; }
        public Vector3Int[] Offsets { get; set; }
    }
}