using System.Collections.Generic;
using Sabotris.Util;

namespace Sabotris.Network.Packets.Game
{
    public class PacketGameEnd : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.GameEnd;
        
        public long Winner { get; set; }
        public Dictionary<long, PlayerScore> Scores { get; set; }

    }
}