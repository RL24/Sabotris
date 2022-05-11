using System;

namespace Sabotris.Network.Packets.Bot
{
    public class PacketBotDisconnected : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.BotDisconnected;

        public Guid BotId { get; set; }
    }
}