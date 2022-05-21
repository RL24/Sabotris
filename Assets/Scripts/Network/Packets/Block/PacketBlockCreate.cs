using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Game
{
    public class PacketBlockCreate : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.BlockCreate;

        public Guid ContainerId { get; set; }
        public Guid Id { get; set; }
        public Vector3Int Position { get; set; }
        public Color Color { get; set; }
    }
}