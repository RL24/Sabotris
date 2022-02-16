using System;
using System.Runtime.InteropServices;
using Network;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using Steamworks;

namespace UI.Menu.Menus
{
    public class Client : Networker
    {
        public event EventHandler<uint> OnLobbiesFetchedEvent;
        public event EventHandler<bool> OnConnectedToLobbyEvent;
        public event EventHandler<DisconnectReason> OnDisconnectedFromLobbyEvent;
        
        public event EventHandler<bool> OnConnectedToServerEvent;
        public event EventHandler<DisconnectReason> OnDisconnectedFromServerEvent;

        private readonly HSteamNetConnection _localConnection = new HSteamNetConnection(0);
        private HSteamNetConnection? _connection;
        
        public bool IsHosting { get; set; }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected || IsHosting;
            set => _isConnected = value;
        }

        public CSteamID? LobbyId { get; set; }
        
        public static ulong UserId => SteamUser.GetSteamID().m_SteamID;
        public static string Username => SteamFriends.GetPersonaName();
        
        public Client(NetworkController networkController) : base(networkController, PacketDirection.Client)
        {
            PacketHandler.Register(this);
            
            Logging.Log(false, "Registering callbacks");
            Callback<LobbyMatchList_t>.Create(LobbiesFetchedCallback);
            Callback<LobbyEnter_t>.Create(LobbyEnteredCallback);
            Callback<SteamNetConnectionStatusChangedCallback_t>.Create(ConnectionStatusChangedCallback);

            OnConnectedToServerEvent += OnConnectedToServer;
        }

        private void OnConnectedToServer(object sender, bool success)
        {
            if (success)
                SendPacket(new PacketRetrievePlayerList());
        }

        #region Lobbies

        public void RequestLobbyList()
        {
            Logging.Log(false, "Requesting all lobbies");
            SteamMatchmaking.RequestLobbyList();
        }

        public void JoinLobby(CSteamID lobbyId)
        {
            if (IsHosting)
            {
                Logging.Log(false, "Running client on host, not joining own lobby but sending fake event");
                LobbyEnteredCallback(new LobbyEnter_t
                {
                    m_ulSteamIDLobby = UserId,
                    m_EChatRoomEnterResponse = (uint) EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess
                });
                return;
            }

            Logging.Log(false, "Joining lobby {0}", lobbyId);
            SteamMatchmaking.JoinLobby(lobbyId);
        }

        public void LeaveLobby()
        {
            if (IsHosting)
            {
                Logging.Log(false, "Running client on host, not leaving own lobby but sending fake event");
                OnDisconnectedFromLobbyEvent?.Invoke(this, DisconnectReason.ClientLeftLobby);
                return;
            }
            
            if (LobbyId == null)
                return;
            
            SteamMatchmaking.LeaveLobby(LobbyId.Value);
            LobbyId = null;
        }
        
        private void LobbiesFetchedCallback(LobbyMatchList_t lobbyMatchListT)
        {
            Logging.Log(false, "Fetched {0} lobbies", lobbyMatchListT.m_nLobbiesMatching);

            OnLobbiesFetchedEvent?.Invoke(this, lobbyMatchListT.m_nLobbiesMatching);
        }

        private void LobbyEnteredCallback(LobbyEnter_t param)
        {
            if ((EChatRoomEnterResponse) param.m_EChatRoomEnterResponse !=
                EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
            {
                Logging.Log(false, "Failed to join lobby: {0}",
                    (EChatRoomEnterResponse) param.m_EChatRoomEnterResponse);
                OnConnectedToLobbyEvent?.Invoke(this, false);
                return;
            }

            Logging.Log(false, "Joined lobby, getting host ID and creating socket P2P connection");
            OnConnectedToLobbyEvent?.Invoke(this, true);

            LobbyId = param.m_ulSteamIDLobby.ToSteamID();
            CreateSocket(LobbyId.Value);
        }
        
        #endregion

        #region Sockets

        private void CreateSocket(CSteamID lobbyId)
        {
            if (IsHosting)
            {
                _connection = _localConnection;
                Logging.Log(false, "Running client on host, not creating socket but sending fake event");
                var networkIdentity = new SteamNetworkingIdentity
                {
                    m_eType = ESteamNetworkingIdentityType.k_ESteamNetworkingIdentityType_SteamID
                };
                networkIdentity.SetSteamID(UserId.ToSteamID());
                NetworkController.Server.OnConnectionStatusChanged(new SteamNetConnectionStatusChangedCallback_t
                {
                    m_info = new SteamNetConnectionInfo_t
                    {
                        m_identityRemote = networkIdentity,
                        m_eState = ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected
                    },
                    m_hConn = new HSteamNetConnection
                    {
                        m_HSteamNetConnection = 0
                    }
                });
                IsConnected = true;
                OnConnectedToServerEvent?.Invoke(this, true);
                return;
            }
            
            var hostIdString = SteamMatchmaking.GetLobbyData(lobbyId, HostIdKey);
            if (!ulong.TryParse(hostIdString, out var hostId))
            {
                Logging.Log(false, "Failed to parse host ID {0}, leaving lobby", hostIdString);
                SteamMatchmaking.LeaveLobby(lobbyId);
                OnDisconnectedFromLobbyEvent?.Invoke(this, DisconnectReason.LobbyHostIdNotFound);
                return;
            }
            
            var identity = new SteamNetworkingIdentity()
            {
                m_eType = ESteamNetworkingIdentityType.k_ESteamNetworkingIdentityType_SteamID
            };
            identity.SetSteamID(hostId.ToSteamID());

            Logging.Log(false, "Connecting P2P...");
            _connection = SteamNetworkingSockets.ConnectP2P(ref identity, 0, 0, new SteamNetworkingConfigValue_t[0]);
        }

        public void DisconnectSocket(DisconnectReason? reason = null)
        {
            IsConnected = false;
            if (IsHosting)
            {
                Logging.Log(false, "Running client on host, not destroying socket but sending fake event");
                IsConnected = IsHosting = false;
                OnDisconnectedFromServerEvent?.Invoke(this, reason ?? DisconnectReason.None);
                return;
            }
            
            LeaveLobby();
            
            if (_connection == null)
                return;
            
            Logging.Log(false, "Closing connection to server");
            
            SteamNetworkingSockets.CloseConnection(_connection.Value, 0, "", false);
            _connection = null;
            OnDisconnectedFromServerEvent?.Invoke(this, reason ?? DisconnectReason.None);
        }

        private void ConnectionStatusChangedCallback(SteamNetConnectionStatusChangedCallback_t param)
        {
            if (IsHosting) // this will be called when hosting the server too for client disconnects, we don't want that. this method is supposed to be for self client connect/disconnect from server only
                return;
            
            switch (param.m_info.m_eState)
            {
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
                {
                    IsConnected = true;
                    OnConnectedToServerEvent?.Invoke(this, true);
                } break;

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Dead:
                {
                    Logging.Log(false, "Connection fully closed");
                    IsConnected = false;
                    OnDisconnectedFromServerEvent?.Invoke(this, DisconnectReason.ConnectionClosed);
                } break;

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
                {
                    DisconnectSocket(DisconnectReason.ConnectionIssue);
                } break;
                
                default:
                    Logging.Log(false, "Connection state changed: {0}", param.m_info.m_eState);
                    break;
            }
        }

        public void PollMessages()
        {
            if (_connection == null || _connection == _localConnection)
                return;
                
            var receivedMessages = new IntPtr[100];
            var incomingMessages =
                SteamNetworkingSockets.ReceiveMessagesOnConnection(_connection.Value, receivedMessages, 100);
            if (incomingMessages == -1)
            {
                Logging.Log(false, "Polling failed, invalid connection");
                return;
            }
            if (incomingMessages <= 0)
                return;

            Logging.Log(false, "Received {0} messages", incomingMessages);
            for (var i = 0; i < incomingMessages; i++)
            {
                var receivedMessage = receivedMessages[i];
                var parsedMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(receivedMessage);
                
                var packet = GetPacket(parsedMessage);
                
                SteamNetworkingMessage_t.Release(receivedMessage);
                
                PacketHandler.Process(packet);
            }
        }
        
        #endregion
        
        #region Sending Packets

        public void SendPacket(Packet packet)
        {
            if (IsHosting)
            {
                Logging.Log(false, "Running client on host, sending {0} packet through flow rather than network", packet.GetPacketType().Id);
                packet.Connection = _localConnection;
                packet.SenderId = UserId;
                NetworkController.Server.PacketHandler.Process(packet);
                return;
            }

            if (_connection == null || _connection == _localConnection)
                return;

            var data = packet.Serialize().Bytes;
            var buffer = SteamNetworkingUtils.AllocateMessage(data.Length);
            Marshal.Copy(data, 0, buffer, data.Length);
            
            var res = SteamNetworkingSockets.SendMessageToConnection(_connection.Value, buffer, (uint) data.Length, 0, out _);
            if (res != EResult.k_EResultOK)
                Logging.Log(false, "Failed to send packet to server: {0}", res);
        }

        #endregion

        #region Packet Listeners
        
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