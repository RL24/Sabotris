using System;

namespace Sabotris.Network.Packets.Bot
{
    public class PacketBotConnected : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.BotConnected;

        public Player Bot { get; set; }
    }
}