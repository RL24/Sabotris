namespace Sabotris.Network.Packets.Game
{
    public class PacketGameStart : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.GameStart;
    }
}