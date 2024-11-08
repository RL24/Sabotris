﻿using System;

namespace Sabotris.Network.Packets.Layer
{
    public class PacketLayerMove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.LayerMove;

        public Guid ContainerId { get; set; }
        public int[] Layers { get; set; }
        public int Y { get; set; }
    }
}