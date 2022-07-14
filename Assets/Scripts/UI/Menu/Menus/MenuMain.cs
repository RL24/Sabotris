using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sabotris.Network;
using Sabotris.Util;
using Steamworks;
using UnityEngine;
using Random = Sabotris.Util.Random;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuMain : Menu
    {
        
        private readonly Vector3 _cameraPosition = new Vector3(-4.5f, 7.5f, -10);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(30, 30, 4);

        public MenuButton buttonQuickPlay,
            buttonHost,
            buttonJoin,
            buttonSettings,
            buttonExit;

        public MenuButton buttonDiscord,
            buttonTwitter;

        private Coroutine _loadingLeaderboard;
        public RectTransform leaderboard;
        public MenuLeaderboardItem leaderboardItem;
        private readonly List<MenuLeaderboardItem> _leaderboardItems = new List<MenuLeaderboardItem>();

        public Menu menuLobby, menuHostGame, menuJoinGame, menuSettings;

        protected override void Start()
        {
            base.Start();

            _loadingLeaderboard = StartCoroutine(LoadLeaderboard());
            
            buttonDiscord.OnClick += OnClickButton;
            buttonTwitter.OnClick += OnClickButton;

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            audioController.music.pitch = 1;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            StopCoroutine(_loadingLeaderboard);

            buttonDiscord.OnClick -= OnClickButton;
            buttonTwitter.OnClick -= OnClickButton;
            
            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonDiscord))
                Application.OpenURL("https://discord.gg/Ef2NDZZJfH");
            else if (sender.Equals(buttonTwitter))
                Application.OpenURL("https://twitter.com/Sabotris");
            else if (sender.Equals(buttonQuickPlay))
                StartCoroutine(QuickPlay());
            else if (sender.Equals(buttonHost))
                menuController.OpenMenu(menuHostGame);
            else if (sender.Equals(buttonJoin))
                menuController.OpenMenu(menuJoinGame);
            else if (sender.Equals(buttonSettings))
                menuController.OpenMenu(menuSettings);
            else if (sender.Equals(buttonExit))
                Application.Quit();
        }

        private IEnumerator QuickPlay()
        {
            SetButtonsDisabled();

            var lobbyCount = new Atomic<uint>(0);
            var lobbiesFetchComplete = false;
            void LobbiesFetched(object sender, uint count)
            {
                if (networkController.Client != null)
                    networkController.Client.OnLobbiesFetchedEvent -= LobbiesFetched;

                lobbyCount.Value = count;
                lobbiesFetchComplete = true;
            }
            
            if (networkController.Client != null)
                networkController.Client.OnLobbiesFetchedEvent += LobbiesFetched;
            
            Client.RequestLobbyList();

            yield return new WaitUntil(() => lobbiesFetchComplete);

            var lobbyId = SteamMatchmakingUtil.GetFirstAvailableLobby(lobbyCount.Value);
            if (lobbyId != null)
            {
                if (networkController.Client == null)
                {
                    SetButtonsDisabled(false);
                    yield break;
                }

                void ConnectedToServer(object sender, HSteamNetConnection? connection)
                {
                    if (networkController.Client != null)
                    {
                        networkController.Client.OnConnectedToServerEvent -= ConnectedToServer;
                        networkController.Client.OnFailedToConnectToServerEvent -= FailedToConnect;
                    }
            
                    if (connection != null)
                        menuController.OpenMenu(menuLobby);
                }
            
                void FailedToConnect(object sender, EventArgs args)
                {
                    if (networkController.Client == null)
                    {
                        SetButtonsDisabled(false);
                        return;
                    }

                    networkController.Client.OnConnectedToServerEvent -= ConnectedToServer;
                    networkController.Client.OnFailedToConnectToServerEvent -= FailedToConnect;
                }
            
                networkController.Client.OnConnectedToServerEvent += ConnectedToServer;
                networkController.Client.OnFailedToConnectToServerEvent += FailedToConnect;
            
                networkController.Client?.JoinLobby(lobbyId.Value);
                
                yield break;
            }
            
            void ServerStarted(object sender, EventArgs args)
            {
                networkController.Server.OnServerStartEvent -= ServerStarted;
            
                if (networkController.Server.LobbyId == null)
                {
                    Logging.Error(true, "Started server but no lobby ID");
                    SetButtonsDisabled(false);
                    return;
                }
            
                networkController.Client?.JoinLobby(networkController.Server.LobbyId.Value);
                menuController.OpenMenu(menuLobby);
            }
            
            networkController.Server.OnServerStartEvent += ServerStarted;
            networkController.Server?.CreateLobby(new LobbyData
            {
                LobbyName = $"{Client.Username}'s Lobby"
            });
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
        }

        private IEnumerator LoadLeaderboard()
        {
            if (!leaderboard)
                yield break;

            var leaderboardEntries = new Atomic<List<LeaderboardEntry_t>>(new List<LeaderboardEntry_t>());
            yield return SteamLeaderboardsUtil.GetLeaderboardScores(leaderboardEntries);
            
            foreach (var item in _leaderboardItems)
                Destroy(item.gameObject);
            _leaderboardItems.Clear();
            
            foreach (var entry in leaderboardEntries.Value.OrderByDescending((x) => x.m_nScore))
                AddScoreboardEntry(entry);
            
            leaderboard.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, leaderboard.childCount * 40);
        }

        private void AddScoreboardEntry(LeaderboardEntry_t entry)
        {
            var item = Instantiate(leaderboardItem, Vector3.zero, Quaternion.identity, leaderboard.transform);
            item.name = $"ScoreEntry-{entry.m_steamIDUser}";
            item.menu = this;
            item.playerName = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
            item.playerHighscore = $"{entry.m_nScore}";

            _leaderboardItems.Add(item);
        }

        protected override void GoBack()
        {
        }

        public override Vector3 GetCameraPosition()
        {
            return _cameraPosition;
        }

        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }
    }
}