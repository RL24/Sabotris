using Sabotris.Game;

namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerConnected : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerConnected;

        public Player Player { get; set; }
    }
}