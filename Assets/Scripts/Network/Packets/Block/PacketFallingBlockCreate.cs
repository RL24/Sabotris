using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Block
{
    public class PacketFallingBlockCreate : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.FallingBlockCreate;

        public Guid ContainerId { get; set; }
        public Guid Id { get; set; }
        public Vector3Int Position { get; set; }
        public Color Color { get; set; }
    }
}