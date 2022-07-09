using System;
using System.Runtime.InteropServices;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Network.Packets.Players;
using Sabotris.Util;
using Steamworks;

namespace Sabotris.UI.Menu.Menus
{
    public class Client : Networker
    {
        public event EventHandler<uint> OnLobbiesFetchedEvent;

        public event EventHandler<HSteamNetConnection?> OnConnectedToServerEvent;
        public event EventHandler<DisconnectReason> OnDisconnectedFromServerEvent;
        public event EventHandler OnFailedToConnectToServerEvent;

        public static readonly Guid UserId = Guid.NewGuid();
        public static readonly CSteamID SteamId = SteamUser.GetSteamID();
        public static readonly string Username = SteamFriends.GetPersonaName();

        private static readonly HSteamNetConnection LocalConnection = new HSteamNetConnection(0);
        private readonly SteamNetConnectionStatusChangedCallback_t _localConnectionStatus;

        public CSteamID? LobbyId;
        private HSteamNetConnection? _connection;

        private bool IsHosting => NetworkController.Server.Running || NetworkController.Server.Starting || _connection == LocalConnection;
        public bool IsConnected => _connection != null;
        public LobbyData LobbyData = new LobbyData();

        public Client(NetworkController networkController) : base(networkController, PacketDirection.Client)
        {
            PacketHandler.Register(this);

            var localIdentity = new SteamNetworkingIdentity {m_eType = ESteamNetworkingIdentityType.k_ESteamNetworkingIdentityType_SteamID};
            localIdentity.SetSteamID(SteamId);
            _localConnectionStatus = new SteamNetConnectionStatusChangedCallback_t
            {
                m_info = new SteamNetConnectionInfo_t
                {
                    m_identityRemote = localIdentity,
                    m_eState = ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected
                },
                m_hConn = LocalConnection
            };

            Callback<LobbyMatchList_t>.Create(LobbiesFetchedCallback);
            Callback<LobbyEnter_t>.Create(LobbyEnteredCallback);
            Callback<SteamNetConnectionStatusChangedCallback_t>.Create(ConnectionStatusChangedCallback);

            OnConnectedToServerEvent += OnConnectedToServer;
            OnDisconnectedFromServerEvent += OnDisconnectedFromServer;
            OnFailedToConnectToServerEvent += OnFailedToConnectToServer;
        }

        private void OnConnectedToServer(object sender, HSteamNetConnection? connection)
        {
            if (connection != null)
                SendPacket(new PacketRetrievePlayerList());
        }

        private void OnDisconnectedFromServer(object sender, DisconnectReason e)
        {
            LeaveLobby();
            _connection = null;
        }

        private void OnFailedToConnectToServer(object sender, EventArgs e)
        {
            LeaveLobby();
            _connection = null;
        }

        public static void RequestLobbyList()
        {
            SteamMatchmaking.RequestLobbyList();
        }

        private void LobbiesFetchedCallback(LobbyMatchList_t lobbyMatchListT)
        {
            OnLobbiesFetchedEvent?.Invoke(this, lobbyMatchListT.m_nLobbiesMatching);
        }

        public void JoinLobby(CSteamID lobbyId)
        {
            if (IsHosting)
            {
                LobbyId = lobbyId;
                _connection = LocalConnection;
                NetworkController.Server.OnConnectionStatusChanged(_localConnectionStatus);
                return;
            }

            SteamMatchmaking.JoinLobby(lobbyId);
        }

        private void LobbyEnteredCallback(LobbyEnter_t param)
        {
            if (IsHosting || IsConnected)
                return;
            
            if ((EChatRoomEnterResponse) param.m_EChatRoomEnterResponse != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
            {
                Logging.Log(false, "Failed to join lobby: {0}", (EChatRoomEnterResponse) param.m_EChatRoomEnterResponse);
                OnFailedToConnectToServerEvent?.Invoke(this, null);
                return;
            }

            LobbyId = param.m_ulSteamIDLobby.ToSteamID();
            CreateSocket(LobbyId.Value);
        }

        private void CreateSocket(CSteamID lobbyId)
        {
            LobbyData = new LobbyData();
            LobbyData.Retrieve(lobbyId);

            if (LobbyData.HostId == null)
            {
                // This should never happen, but in the off chance it does
                Logging.Log(false, "Failed to parse host ID, leaving lobby");
                OnFailedToConnectToServerEvent?.Invoke(this, null);
                return;
            }

            var identity = new SteamNetworkingIdentity()
            {
                m_eType = ESteamNetworkingIdentityType.k_ESteamNetworkingIdentityType_SteamID
            };
            identity.SetSteamID(LobbyData.HostId.Value);
            SteamNetworkingSockets.ConnectP2P(ref identity, 0, 0, new SteamNetworkingConfigValue_t[0]);
        }

        private void ConnectionStatusChangedCallback(SteamNetConnectionStatusChangedCallback_t param)
        {
            if (IsHosting)
                return;

            switch (param.m_info.m_eState)
            {
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
                    _connection = param.m_hConn;
                    break;

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Dead:
                    DisconnectSocket(DisconnectReason.ConnectionClosed);
                    break;

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
                    DisconnectSocket(DisconnectReason.ServerClosed);
                    break;

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
                    DisconnectSocket(DisconnectReason.ConnectionIssue);
                    break;
            }
        }

        private void LeaveLobby()
        {
            if (LobbyId == null)
                return;

            SteamMatchmaking.LeaveLobby(LobbyId.Value);
            LobbyId = null;
        }

        public void DisconnectSocket(DisconnectReason? reason = null)
        {
            reason ??= DisconnectReason.None;
            if (IsHosting)
            {
                OnDisconnectedFromServerEvent?.Invoke(this, reason.Value);
                return;
            }

            if (_connection == null)
            {
                OnDisconnectedFromServerEvent?.Invoke(this, reason.Value);
                return;
            }

            SteamNetworkingSockets.CloseConnection(_connection.Value, 0, "", false);
            OnDisconnectedFromServerEvent?.Invoke(this, reason.Value);
        }

        public void PollMessages()
        {
            if (_connection == null || _connection == LocalConnection)
                return;

            var receivedMessages = new IntPtr[100];
            var incomingMessages = SteamNetworkingSockets.ReceiveMessagesOnConnection(_connection.Value, receivedMessages, 100);
            ProcessIncomingMessages(receivedMessages, incomingMessages);
        }

        #region Sending Packets

        public void SendPacket(Packet packet)
        {
            if (_connection == null)
                return;

            if (IsHosting)
            {
                packet.Connection = _connection;
                packet.SenderId = SteamId.m_SteamID;
                NetworkController.Server.PacketHandler.Process(packet);
                return;
            }

            var data = packet.Serialize().Bytes;
            var message = Marshal.PtrToStructure<SteamNetworkingMessage_t>(SteamNetworkingUtils.AllocateMessage(data.Length));
            Marshal.Copy(data, 0, message.m_pData, data.Length);
            SendNetworkMessage(_connection.Value, message, (uint) data.Length);
        }

        #endregion

        #region Packet Listeners

        [PacketListener(PacketTypeId.RetrievePlayerId, PacketDirection.Client)]
        public void OnPacketRetrievePlayerId(PacketRetrievePlayerId packet)
        {
            packet.Id = UserId;
            SendPacket(packet);
            OnConnectedToServerEvent?.Invoke(this, _connection);
        }

        [PacketListener(PacketTypeId.ServerShutdown, PacketDirection.Client)]
        public void OnPacketServerShutdown(PacketServerShutdown packet)
        {
            LeaveLobby();
            DisconnectSocket(DisconnectReason.ServerClosed);
        }
        
        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnPacketGameStart(PacketGameStart packet)
        {
            LeaveLobby();
        }

        #endregion
    }
}