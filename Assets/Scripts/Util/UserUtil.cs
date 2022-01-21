using UnityEngine;

namespace Sabotris.Util
{
    public static class UserUtil
    {
        public static string GenerateUsername()
        {
            var username = "";
            for (var i = 0; i < 6; i++)
                username += "abcdefghijklmnopqrstuvwxyz"[Random.Range(0, 26)];
            return username;
        }
    }
}