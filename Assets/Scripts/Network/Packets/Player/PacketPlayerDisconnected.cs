﻿using System;

namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerDisconnected : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerDisconnected;

        public Guid Id { get; set; }
    }
}