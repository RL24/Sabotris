﻿using Sabotris.Game;

namespace Sabotris.Network.Packets.Game
{
    public class PacketPlayerList : Packet
    {
        public override PacketType GetPacketType() => PacketTypes.PlayerList;

        public Player[] Players { get; set; }
        public Player[] Bots { get; set; }
    }
}