using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Game
{
    public class PacketFallingBlockCreate : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.FallingBlockCreate;

        public ulong ContainerId { get; set; }
        public Guid Id { get; set; }
        public Vector3Int Position { get; set; }
        public Color Color { get; set; }
    }
}