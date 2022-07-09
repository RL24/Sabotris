using System.Globalization;
using Sabotris.UI.Menu.Menus;
using Steamworks;
using UnityEngine;

namespace Sabotris.Network
{
    public class LobbyData
    {
        private const string HostIdKey = "HostId";
        private const string LobbyNameKey = "LobbyName";
        private const string LobbyPlayerCountKey = "PlayerCount";

        private const string BotCountKey = "BotCount";
        private const string BotDifficultyKey = "BotDifficulty";
        private const string PlayFieldSizeKey = "PlayFieldSize";
        private const string MaxPlayersKey = "MaxPlayers";
        private const string BlocksPerShapeKey = "BlocksPerShape";
        private const string GenerateVerticalBlocksKey = "GenerateVerticalBlocks";
        private const string PracticeModeKey = "PracticeMode";
        private const string PowerUpsKey = "PowerUps";
        private const string PowerUpAutoPickDelayKey = "PowerUpAutoPickDelay";

        public CSteamID? HostId;
        public string LobbyName;
        public int PlayerCount;
        public int BotCount;
        public int BotDifficulty = 5;
        public int PlayFieldSize = 2;
        public int MaxPlayers = 4;
        public int BlocksPerShape = 4;
        public bool GenerateVerticalBlocks;
        public bool PracticeMode;
        public bool PowerUps = true;
        public float PowerUpAutoPickDelay = 5f;

        private void ParseHostId(string hostId)
        {
            if (ulong.TryParse(hostId, out var hostIdParsed))
                HostId = new CSteamID(hostIdParsed);
            else HostId = null;
        }

        private void ParseBotCount(string botCount)
        {
            int.TryParse(botCount, out BotCount);
        }

        private void ParseBotDifficulty(string botDifficulty)
        {
            int.TryParse(botDifficulty, out BotDifficulty);
        }

        private void ParsePlayFieldSize(string playFieldSize)
        {
            int.TryParse(playFieldSize, out PlayFieldSize);
        }

        private void ParseMaxPlayers(string maxPlayers)
        {
            int.TryParse(maxPlayers, out MaxPlayers);
        }

        private void ParsePlayerCount(string playerCount)
        {
            int.TryParse(playerCount, out PlayerCount);
        }

        private void ParseBlocksPerShape(string blocksPerShape)
        {
            int.TryParse(blocksPerShape, out BlocksPerShape);
        }

        private void ParseGenerateVerticalBlocks(string generateVerticalBlocks)
        {
            bool.TryParse(generateVerticalBlocks, out GenerateVerticalBlocks);
        }

        private void ParsePracticeMode(string practiceMode)
        {
            bool.TryParse(practiceMode, out PracticeMode);
        }

        private void ParsePowerUps(string powerUps)
        {
            bool.TryParse(powerUps, out PowerUps);
        }

        private void ParsePowerUpAutoPickDelay(string powerUpAutoPickDelay)
        {
            float.TryParse(powerUpAutoPickDelay, out PowerUpAutoPickDelay);
        }

        public void Store(CSteamID? lobbyId)
        {
            if (lobbyId == null)
                return;

            SteamMatchmaking.SetLobbyData(lobbyId.Value, HostIdKey, Client.SteamId.m_SteamID.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, LobbyNameKey, LobbyName);
            SteamMatchmaking.SetLobbyData(lobbyId.Value, LobbyPlayerCountKey, PlayerCount.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, BotCountKey, BotCount.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, BotDifficultyKey, BotDifficulty.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, PlayFieldSizeKey, PlayFieldSize.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, MaxPlayersKey, MaxPlayers.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, BlocksPerShapeKey, BlocksPerShape.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, GenerateVerticalBlocksKey, GenerateVerticalBlocks.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, PracticeModeKey, PracticeMode.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, PowerUpsKey, PowerUps.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, PowerUpAutoPickDelayKey, PowerUpAutoPickDelay.ToString(CultureInfo.InvariantCulture));
        }

        public void Retrieve(CSteamID? lobbyId)
        {
            if (lobbyId == null)
                return;

            ParseHostId(SteamMatchmaking.GetLobbyData(lobbyId.Value, HostIdKey));
            LobbyName = SteamMatchmaking.GetLobbyData(lobbyId.Value, LobbyNameKey);
            ParsePlayerCount(SteamMatchmaking.GetLobbyData(lobbyId.Value, LobbyPlayerCountKey));
            ParseBotCount(SteamMatchmaking.GetLobbyData(lobbyId.Value, BotCountKey));
            ParseBotDifficulty(SteamMatchmaking.GetLobbyData(lobbyId.Value, BotDifficultyKey));
            ParsePlayFieldSize(SteamMatchmaking.GetLobbyData(lobbyId.Value, PlayFieldSizeKey));
            ParseMaxPlayers(SteamMatchmaking.GetLobbyData(lobbyId.Value, MaxPlayersKey));
            ParseBlocksPerShape(SteamMatchmaking.GetLobbyData(lobbyId.Value, BlocksPerShapeKey));
            ParseGenerateVerticalBlocks(SteamMatchmaking.GetLobbyData(lobbyId.Value, GenerateVerticalBlocksKey));
            ParsePracticeMode(SteamMatchmaking.GetLobbyData(lobbyId.Value, PracticeModeKey));
            ParsePowerUps(SteamMatchmaking.GetLobbyData(lobbyId.Value, PowerUpsKey));
            ParsePowerUpAutoPickDelay(SteamMatchmaking.GetLobbyData(lobbyId.Value, PowerUpAutoPickDelayKey));
        }

        public void UpdatePlayerCount(CSteamID? lobbyId, int playerCount)
        {
            if (lobbyId == null)
                return;

            PlayerCount = playerCount;
            SteamMatchmaking.SetLobbyData(lobbyId.Value, LobbyPlayerCountKey, PlayerCount.ToString());
        }
    }
}