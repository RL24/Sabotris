using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Spectator
{
    public class PacketSpectatorMove : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.SpectatorMove;
        
        public Guid Id { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    } 
}