using System;

namespace Sabotris.Network.Packets.Game
{
    public class PacketRetrievePlayerId : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.RetrievePlayerId;

        public Guid Id { get; set; }
        public ulong SteamId { get; set; }
        public bool IsNewConnection { get; set; }
    }
}