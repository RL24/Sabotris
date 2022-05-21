using System;

namespace Sabotris.Network.Packets.Game
{
    public class PacketLayerAdd : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.LayerAdd;

        public Guid ContainerId { get; set; }
        public int Layer { get; set; }
    }
}