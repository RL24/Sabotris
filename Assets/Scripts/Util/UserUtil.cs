using System;
using UnityEngine;

namespace Sabotris.Util
{
    public static class UserUtil
    {
        private static readonly System.Random Random = new System.Random();
        
        public static string GenerateUsername()
        {
            var username = "";
            for (var i = 0; i < 6; i++)
                username += "abcdefghijklmnopqrstuvwxyz"[Random.Next(0, 26)];
            return username;
        }
    }
}