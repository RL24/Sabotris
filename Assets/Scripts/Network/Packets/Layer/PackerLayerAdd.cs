using System;

namespace Sabotris.Network.Packets.Layer
{
    public class PacketLayerAdd : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.LayerAdd;

        public Guid ContainerId { get; set; }
        public int Layer { get; set; }
    }
}