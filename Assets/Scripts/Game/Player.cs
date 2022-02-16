
namespace Sabotris
{
    public class PlayerScore
    {
        public int Score { get; set; }
        public int ClearedLayers { get; set; }

        public PlayerScore(int score, int clearedLayers)
        {
            Score = score;
            ClearedLayers = clearedLayers;
        }
    }
    
    public class Player
    {
        public ulong Id { get; set; }
        public string Name { get; private set; }

        public Player(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}