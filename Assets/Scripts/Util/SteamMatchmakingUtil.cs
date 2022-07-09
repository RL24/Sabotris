using System;
using System.Collections;
using Sabotris.Network;
using Steamworks;
using UnityEngine;

namespace Sabotris.Util
{
    public static class SteamMatchmakingUtil
    {
        public static CSteamID? GetFirstAvailableLobby(uint lobbyCount)
        {
            for (var i = 0; i < lobbyCount; i++)
            {
                var lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
                var lobbyData = new LobbyData();
                lobbyData.Retrieve(lobbyId);
                
                if (lobbyData.PlayerCount < lobbyData.MaxPlayers)
                    return lobbyId;
            }

            return null;
        }
    }
}