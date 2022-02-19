namespace Sabotris.Network.Packets.Game
{
    public class PacketLayerMove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.LayerMove;

        public ulong ContainerId { get; set; }
        public int[] Layers { get; set; }
    }
}