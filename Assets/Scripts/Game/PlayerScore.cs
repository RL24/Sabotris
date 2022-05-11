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
}