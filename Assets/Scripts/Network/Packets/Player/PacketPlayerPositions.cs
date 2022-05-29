using System;
using UnityEngine;

namespace Sabotris.Network.Packets.Players
{
    public class PacketPlayerPositions : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerPositions;

        public (Guid, Vector3)[] Positions { get; set; }
    }
}