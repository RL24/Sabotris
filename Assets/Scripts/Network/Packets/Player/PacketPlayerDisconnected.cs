namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerDisconnected : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerDisconnected;
        
        public ulong Id { get; set; }
    }
}