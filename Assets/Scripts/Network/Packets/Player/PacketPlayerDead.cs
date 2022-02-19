namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerDead : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerDead;

        public ulong Id { get; set; }
    }
}