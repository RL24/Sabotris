using System;
using System.Collections;
using System.Collections.Generic;
using Sabotris.Network;
using Steamworks;
using UnityEngine;

namespace Sabotris.Util
{
    public static class SteamMatchmakingUtil
    {
        public static event EventHandler<LobbyMatchList_t> OnLobbiesFetched;
        public static event EventHandler<LobbyCreated_t> OnLobbyCreated;
        public static event EventHandler<LobbyEnter_t> OnLobbyJoined;

        static SteamMatchmakingUtil() {
            Callback<LobbyMatchList_t>.Create(LobbiesFetched);
            Callback<LobbyCreated_t>.Create(LobbyCreated);
            Callback<LobbyEnter_t>.Create(LobbyJoined);
        }

        private static void LobbiesFetched(LobbyMatchList_t lobbyMatchList)
        {
            OnLobbiesFetched?.Invoke(null, lobbyMatchList);
        }

        private static void LobbyCreated(LobbyCreated_t lobbyCreated)
        {
            OnLobbyCreated?.Invoke(null, lobbyCreated);
        }

        private static void LobbyJoined(LobbyEnter_t lobbyEnter)
        {
            OnLobbyJoined?.Invoke(null, lobbyEnter);
        }

        public static IEnumerable GetLobbies(Atomic<uint> lobbyCount)
        {
            var hasLobbies = false;
            void LocalLobbiesFetched(object sender, LobbyMatchList_t lobbyMatchList)
            {
                lobbyCount.Value = lobbyMatchList.m_nLobbiesMatching;
                hasLobbies = true;
            }
            OnLobbiesFetched += LocalLobbiesFetched;
            SteamMatchmaking.RequestLobbyList();
            yield return new WaitUntil(() => hasLobbies);
            OnLobbiesFetched -= LocalLobbiesFetched;
        }

        public static CSteamID? GetFirstAvailableLobby(uint lobbyCount)
        {
            for (var i = 0; i < lobbyCount; i++)
            {
                var lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
                var playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyId);
                var lobbyData = new LobbyData();
                lobbyData.Retrieve(lobbyId);

                if (playerCount < lobbyData.MaxPlayers)
                    return lobbyId;
            }

            return null;
        }
    }
}