using Steamworks;

namespace Sabotris.Util
{
    public static class UserUtil
    {
        public static string GenerateUsername()
        {
            return SteamFriends.GetPersonaName();
        }
    }
}