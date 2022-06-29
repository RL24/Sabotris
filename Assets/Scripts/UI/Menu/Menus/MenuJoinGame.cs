using System;
using System.Collections.Generic;
using Sabotris.Network;
using Sabotris.Translations;
using Sabotris.Util;
using Steamworks;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuJoinGame : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(5, 11, -3.5f);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(55, -51, -7);

        public MenuLobbyListItem lobbyListItemTemplate;

        public RectTransform lobbyList;
        public MenuButton buttonRefresh, buttonBack;

        public Menu menuMain, menuLobby;

        private bool _requestingLobbies;

        private bool RequestingLobbies
        {
            get => _requestingLobbies;
            set
            {
                if (value == _requestingLobbies)
                    return;

                _requestingLobbies = value;

                buttonRefresh.isDisabled = RequestingLobbies;
            }
        }

        private readonly Dictionary<ulong, MenuLobbyListItem> _lobbies = new Dictionary<ulong, MenuLobbyListItem>();

        protected override void Start()
        {
            base.Start();

            if (networkController.Client != null)
                networkController.Client.OnLobbiesFetchedEvent += OnLobbiesFetched;

            RefreshLobbies();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;

            _lobbies.Clear();

            if (networkController.Client != null)
                networkController.Client.OnLobbiesFetchedEvent -= OnLobbiesFetched;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonRefresh))
                RefreshLobbies();
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        private void OnClickLobbyItem(object sender, EventArgs args)
        {
            if (sender is MenuLobbyListItem lobbyListItem)
                JoinLobby(lobbyListItem.lobbyId);
        }

        private void RefreshLobbies()
        {
            if (RequestingLobbies)
                return;

            audioController.refreshingLobbies.loop = true;
            audioController.refreshingLobbies.Play();

            RequestingLobbies = true;
            foreach (var lobby in _lobbies.Values)
                Destroy(lobby.gameObject);
            _lobbies.Clear();

            AddNoticeMessage(Localization.Translate(TranslationKey.UiMenuNoticeRefreshing));
            Client.RequestLobbyList();
        }

        private void OnLobbiesFetched(object sender, uint lobbyCount)
        {
            audioController.refreshingLobbies.loop = false;

            RequestingLobbies = false;
            foreach (var lobby in _lobbies.Values)
                Destroy(lobby.gameObject);
            _lobbies.Clear();

            if (lobbyCount == 0)
            {
                AddNoticeMessage(Localization.Translate(TranslationKey.UiMenuNoticeNoLobbies));
                return;
            }

            for (var i = 0; i < lobbyCount; i++)
            {
                var lobbyId = SteamMatchmaking.GetLobbyByIndex(i);

                var lobbyData = new LobbyData();
                lobbyData.Retrieve(lobbyId);

                if (!lobbyData.PracticeMode)
                    AddServerEntry(lobbyId.m_SteamID, lobbyData);
            }

            if (_lobbies.Count == 0)
                AddNoticeMessage(Localization.Translate(TranslationKey.UiMenuNoticeNoLobbies));
        }

        private void AddNoticeMessage(string message)
        {
            AddServerEntry(0, new LobbyData
            {
                LobbyName = message,
                PlayerCount = -1
            });
        }

        private void AddServerEntry(ulong lobbyId, LobbyData lobbyData)
        {
            var lobbyListItem = Instantiate(lobbyListItemTemplate, Vector3.zero, Quaternion.identity, lobbyList.transform);
            lobbyListItem.name = $"Server-{lobbyData.LobbyName}-{lobbyId}";
            lobbyListItem.menu = this;
            lobbyListItem.lobbyId = lobbyId;
            lobbyListItem.LobbyName = lobbyData.LobbyName;
            lobbyListItem.LobbyPlayerCount = (lobbyData.PlayerCount == -1 ? (int?) null : lobbyData.PlayerCount);
            lobbyListItem.MaxLobbyPlayers = lobbyData.MaxPlayers;

            if (lobbyId != 0 && lobbyData.PlayerCount <= lobbyData.MaxPlayers)
            {
                buttons.Add(lobbyListItem);
                lobbyListItem.OnClick += OnClickLobbyItem;
            }

            _lobbies.Add(lobbyId, lobbyListItem);

            lobbyList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lobbyList.childCount * 60);
        }

        private void JoinLobby(ulong lobbyId)
        {
            SetButtonsDisabled();

            void ConnectedToServer(object sender, HSteamNetConnection? connection)
            {
                if (networkController.Client != null)
                {
                    networkController.Client.OnConnectedToServerEvent -= ConnectedToServer;
                    networkController.Client.OnFailedToConnectToServerEvent -= FailedToConnect;
                }

                if (connection != null)
                    menuController.OpenMenu(menuLobby);
                else
                    SetButtonsDisabled(false);
            }

            void FailedToConnect(object sender, EventArgs args)
            {
                if (networkController.Client != null)
                {
                    networkController.Client.OnConnectedToServerEvent -= ConnectedToServer;
                    networkController.Client.OnFailedToConnectToServerEvent -= FailedToConnect;
                }
                SetButtonsDisabled(false);
                RefreshLobbies();
            }

            if (networkController.Client != null)
            {
                networkController.Client.OnConnectedToServerEvent += ConnectedToServer;
                networkController.Client.OnFailedToConnectToServerEvent += FailedToConnect;
            }

            networkController.Client?.JoinLobby(lobbyId.ToSteamID());
        }

        protected override Menu GetBackMenu()
        {
            return menuMain;
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