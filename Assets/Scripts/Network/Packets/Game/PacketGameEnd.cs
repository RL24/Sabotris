namespace Sabotris.Network.Packets.Game
{
    public class PacketGameEnd : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.GameEnd;
    }
}