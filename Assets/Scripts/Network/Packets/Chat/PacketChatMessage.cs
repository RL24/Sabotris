using System;

namespace Sabotris.Network.Packets.Chat
{
    public class PacketChatMessage : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ChatMessage;

        public Guid Id { get; set; }
        public Guid Author { get; set; }
        public string AuthorName { get; set; }
        public string Message { get; set; }
    }
}