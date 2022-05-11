using System;

namespace Sabotris.Game
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; }
        public ulong? SteamId { get; }

        public Player(Guid id, string name, ulong? steamId = null)
        {
            Id = id;
            Name = name;
            SteamId = steamId;
        }
    }
}