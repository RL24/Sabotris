namespace Sabotris.Network.Packets.Game
{
    public class PacketLayerClear : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.LayerClear;

        public ulong ContainerId { get; set; }
        public int Layer { get; set; }
    }
}