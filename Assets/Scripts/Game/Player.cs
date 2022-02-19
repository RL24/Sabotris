namespace Sabotris
{
    public class PlayerScore
    {
        public int Score { get; }
        public int ClearedLayers { get; }

        public PlayerScore(int score, int clearedLayers)
        {
            Score = score;
            ClearedLayers = clearedLayers;
        }
    }

    public class Player
    {
        public ulong Id { get; }
        public string Name { get; }

        public Player(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}