using System;
using System.Collections;
using Sabotris.Network;
using Sabotris.Util;
using Steamworks;
using UnityEngine;

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

        public Menu menuLobby, menuHostGame, menuJoinGame, menuSettings;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            audioController.music.pitch = 1;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonQuickPlay))
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
            networkController.Server?.CreateLobby(new LobbyData());
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
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