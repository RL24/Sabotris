using System;

namespace Sabotris.Network.Packets.Game
{
    public class PacketChatMessage : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.ChatMessage;
        
        public Guid Id { get; set; }
        public ulong Author { get; set; }
        public string Message { get; set; }
    }
}