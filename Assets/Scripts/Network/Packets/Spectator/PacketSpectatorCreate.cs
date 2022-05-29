using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Spectator
{
    public class PacketSpectatorCreate : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.SpectatorCreate;
        
        public Guid Id { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    } 
}