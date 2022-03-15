using Sabotris.UI.Menu.Menus;
using Steamworks;

namespace Sabotris.Network
{
    public class LobbyData
    {
        public const string HostIdKey = "HostId";
        public const string LobbyNameKey = "LobbyName";
        public const string LobbyPlayerCountKey = "PlayerCount";
        public const string MaxPlayersKey = "MaxPlayers";
        public const string BlocksPerShapeKey = "BlocksPerShape";
        public const string GenerateVerticalBlocksKey = "GenerateVerticalBlocks";
        public const string PracticeModeKey = "PracticeMode";

        public CSteamID? HostId;
        public string LobbyName;
        public int PlayerCount;
        public int MaxPlayers = 4;
        public int BlocksPerShape = 4;
        public bool GenerateVerticalBlocks;
        public bool PracticeMode;

        private void ParseHostId(string hostId)
        {
            if (ulong.TryParse(hostId, out var hostIdParsed))
                HostId = new CSteamID(hostIdParsed);
            else HostId = null;
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

        public void Store(CSteamID? lobbyId)
        {
            if (lobbyId == null)
                return;
            
            SteamMatchmaking.SetLobbyData(lobbyId.Value, HostIdKey, Client.UserId.m_SteamID.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, LobbyNameKey, LobbyName);
            SteamMatchmaking.SetLobbyData(lobbyId.Value, LobbyPlayerCountKey, PlayerCount.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, MaxPlayersKey, MaxPlayers.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, BlocksPerShapeKey, BlocksPerShape.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, GenerateVerticalBlocksKey, GenerateVerticalBlocks.ToString());
            SteamMatchmaking.SetLobbyData(lobbyId.Value, PracticeModeKey, PracticeMode.ToString());
        }

        public void Retrieve(CSteamID? lobbyId)
        {
            if (lobbyId == null)
                return;
            
            ParseHostId(SteamMatchmaking.GetLobbyData(lobbyId.Value, HostIdKey));
            LobbyName = SteamMatchmaking.GetLobbyData(lobbyId.Value, LobbyNameKey);
            ParsePlayerCount(SteamMatchmaking.GetLobbyData(lobbyId.Value, LobbyPlayerCountKey));
            ParseMaxPlayers(SteamMatchmaking.GetLobbyData(lobbyId.Value, MaxPlayersKey));
            ParseBlocksPerShape(SteamMatchmaking.GetLobbyData(lobbyId.Value, BlocksPerShapeKey));
            ParseGenerateVerticalBlocks(SteamMatchmaking.GetLobbyData(lobbyId.Value, GenerateVerticalBlocksKey));
            ParsePracticeMode(SteamMatchmaking.GetLobbyData(lobbyId.Value, PracticeModeKey));
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