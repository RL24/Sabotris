namespace Sabotris.Network.Packets.Players
{
    public class PacketRetrievePlayerList : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.RetrievePlayerList;
    }
}