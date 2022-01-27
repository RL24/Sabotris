namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerScore : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerScore;
        
        public long Id { get; set; }
        public int Score { get; set; }
    }
}