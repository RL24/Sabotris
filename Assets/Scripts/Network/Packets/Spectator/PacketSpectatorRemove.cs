using System;

namespace Sabotris.Network.Packets.Spectator
{
    public class PacketSpectatorRemove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.SpectatorRemove;
        
        public Guid Id { get; set; }
    } 
}