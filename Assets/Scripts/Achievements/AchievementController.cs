using Sabotris.UI.Menu.Menus;
using Steamworks;
using UnityEngine;

namespace Sabotris.Achievements
{
    public class AchievementController : MonoBehaviour
    {
        private void Start()
        {
            Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            SteamUserStats.RequestUserStats(Client.SteamId);
        }

        private void OnUserStatsReceived(UserStatsReceived_t userStatsReceived)
        {
        }
        
        public void Achieve(Achievement achievement)
        {
            SteamUserStats.SetAchievement(achievement.ToString());
        }
    }
}