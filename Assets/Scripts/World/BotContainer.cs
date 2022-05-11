using UnityEngine;

namespace Sabotris
{
    public class BotContainer : ControlledContainer
    {
        private const float MinDifficulty = 0;
        private const float MaxDifficulty = 10;

        private float ClampDifficulty => MaxDifficulty - Mathf.Clamp(networkController.Client?.LobbyData?.BotDifficulty ?? 5, MinDifficulty, MaxDifficulty);
        
        protected override float GetScanDelay()
        {
            return ClampDifficulty * 0.1f;
        }

        protected override float GetMinimumMoveDelay()
        {
            return ClampDifficulty * 0.05f;
        }

        protected override float GetMaximumMoveDelay()
        {
            return ClampDifficulty * 0.15f;
        }

        protected override float GetPermaDropDelay()
        {
            return ClampDifficulty * 0.1f;
        }
    }
}