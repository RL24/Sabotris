
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
        public long Id { get; set; }
        public string Name { get; private set; }

        public Player(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}