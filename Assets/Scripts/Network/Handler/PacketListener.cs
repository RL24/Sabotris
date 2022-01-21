using System;

namespace Sabotris.Network.Packets
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PacketListener : Attribute
    {
        public PacketTypeId PacketType { get; }
        public PacketDirection PacketDirection { get; }

        public PacketListener(PacketTypeId packetType, PacketDirection packetDirection)
        {
            PacketType = packetType;
            PacketDirection = packetDirection;
        }
    }
}