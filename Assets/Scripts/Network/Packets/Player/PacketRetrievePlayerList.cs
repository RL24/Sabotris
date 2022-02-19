namespace Sabotris.Network.Packets.Game
{
    public class PacketRetrievePlayerList : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.RetrievePlayerList;
    }
}