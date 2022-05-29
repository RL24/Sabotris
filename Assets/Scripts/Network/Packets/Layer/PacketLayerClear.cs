using System;

namespace Sabotris.Network.Packets.Layer
{
    public class PacketLayerClear : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.LayerClear;

        public Guid ContainerId { get; set; }
        public int Layer { get; set; }
    }
}