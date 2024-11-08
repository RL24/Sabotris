﻿using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Shape
{
    public class PacketFallingShapeCreate : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.FallingShapeCreate;

        public Guid ContainerId { get; set; }
        public Guid Id { get; set; }
        public Vector3Int Position { get; set; }
        public (Guid, Vector3Int)[] Offsets { get; set; }
        public Color Color { get; set; }
    }
}