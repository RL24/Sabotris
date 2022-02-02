using System;
using Steamworks;
using UnityEngine;

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