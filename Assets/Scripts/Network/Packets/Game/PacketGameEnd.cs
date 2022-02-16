using System.Collections.Generic;

namespace Sabotris.Network.Packets.Game
{
    public class PacketGameEnd : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.GameEnd;
        
        public ulong Winner { get; set; }
        public Dictionary<ulong, PlayerScore> Scores { get; set; }

    }
}