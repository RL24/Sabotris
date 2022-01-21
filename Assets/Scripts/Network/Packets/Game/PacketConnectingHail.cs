namespace Sabotris.Network.Packets.Game
{
    public class PacketConnectingHail : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ConnectingHail;
        
        public string Password { get; set; }
        public string Name { get; set; }
    }
}