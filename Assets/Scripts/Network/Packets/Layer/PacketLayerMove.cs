using System;

namespace Sabotris.Network.Packets.Game
{
    public class PacketLayerMove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.LayerMove;

        public Guid ContainerId { get; set; }
        public int[] Layers { get; set; }
    }
}