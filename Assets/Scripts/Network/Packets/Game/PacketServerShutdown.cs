namespace Sabotris.Network.Packets.Game
{
    public class PacketServerShutdown : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ServerShutdown;
    }
}