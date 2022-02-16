using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Network;
using Sabotris;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using Steamworks;

namespace UI.Menu.Menus
{
    public class Server : Networker
    {
        public event EventHandler OnServerStart;
        public event EventHandler<DisconnectReason> OnServerStop;
        
        private readonly Dictionary<ulong, HSteamNetConnection> _playerConnections = new Dictionary<ulong, HSteamNetConnection>();
        private HSteamNetPollGroup _connectionPollGroup;

        private readonly World _world;

        private HSteamListenSocket? _listenSocket;
        private string _lobbyName;
        
        public bool Running { get; set; }
        public CSteamID? LobbyId { get; set; }

        public Server(NetworkController networkController, World world) : base(networkController, PacketDirection.Server)
        {
            _world = world;
            
            PacketHandler.Register(this);

            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdated);
            Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionStatusChanged);
        }

        #region Lobbies

        public void CreateLobby(string lobbyName)
        {
            Logging.Log(true, "Creating lobby: {0}", lobbyName);
            _lobbyName = lobbyName;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
        }

        public void LeaveLobby()
        {
            if (LobbyId == null)
                return;

            Logging.Log(true, "Leaving lobby");
            SteamMatchmaking.LeaveLobby(LobbyId.Value);
            LobbyId = null;
        }

        private void OnLobbyCreated(LobbyCreated_t param)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                Logging.Log(true, "Failed to create lobby: {0}", param.m_eResult);
                return;
            }

            LobbyId = param.m_ulSteamIDLobby.ToSteamID();
            
            Logging.Log(true, "Lobby created: {0}, setting host ID and waiting for players to join", LobbyId);

            SteamMatchmaking.SetLobbyData(LobbyId.Value, HostIdKey, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(LobbyId.Value, LobbyNameKey, _lobbyName);
            
            _connectionPollGroup = SteamNetworkingSockets.CreatePollGroup();
            CreateListenerSocket();
        }

        private void OnLobbyChatUpdated(LobbyChatUpdate_t param)
        {
            var connectedSteamUser = param.m_ulSteamIDUserChanged.ToSteamID();
            var connectedSteamUserName = SteamFriends.GetFriendPersonaName(connectedSteamUser);
            Logging.Log(true, "Lobby chat updated: {0} ({1})", connectedSteamUserName, (EChatMemberStateChange) param.m_rgfChatMemberStateChange);
        }
        
        #endregion
        
        #region Sockets

        public void CreateListenerSocket()
        {
            Logging.Log(true, "Creating P2P socket listener");
            _listenSocket = SteamNetworkingSockets.CreateListenSocketP2P(0, 0, new SteamNetworkingConfigValue_t[0]);

            Running = true;
            OnServerStart?.Invoke(this, null);
        }

        public void DisconnectSockets(DisconnectReason? reason = null)
        {
            Running = false;
            LeaveLobby();
            foreach (var connection in _playerConnections.Values)
                SteamNetworkingSockets.CloseConnection(connection, 0, "", false);
            SteamNetworkingSockets.DestroyPollGroup(_connectionPollGroup);
            if (_listenSocket != null)
                SteamNetworkingSockets.CloseListenSocket(_listenSocket.Value);
            _playerConnections.Clear();
            OnServerStop?.Invoke(this, DisconnectReason.ServerClosed);
        }

        public void OnConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t param)
        {
            if (!Running)
                return;
            
            var steamId = param.m_info.m_identityRemote.GetSteamID();
            var player = new Player(steamId.m_SteamID, SteamFriends.GetFriendPersonaName(steamId));
            switch (param.m_info.m_eState)
            {
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
                {
                    Logging.Log(true, "Socket connection successful, player: {0} ({1})", player.Name, player.Id);
                    _playerConnections.Add(param.m_info.m_identityRemote.GetSteamID().m_SteamID, param.m_hConn);
                    SteamNetworkingSockets.SetConnectionPollGroup(param.m_hConn, _connectionPollGroup);

                    SendToAll(new PacketPlayerConnected
                    {
                        Player = player
                    }, player.Id);
                } break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
                {
                    Logging.Log(true, "Socket connection requested, accepting connection, player: {0} ({1})", player.Name, player.Id);
                    SteamNetworkingSockets.AcceptConnection(param.m_hConn);
                } break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Dead:
                {
                    Logging.Log(true, "Socket connection dead: {0} ({1})", player.Name, player.Id);
                    _playerConnections.Remove(player.Id);
                    SendToAll(new PacketPlayerDisconnected
                    {
                        Id = player.Id
                    }, player.Id);
                } break;
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
                {
                    Logging.Log(true, "Socket connection closed by peer, closing connection: {0} ({1})", player.Name, player.Id);
                    SteamNetworkingSockets.CloseConnection(param.m_hConn, 0, "", false);
                    _playerConnections.Remove(player.Id);
                    SendToAll(new PacketPlayerDisconnected
                    {
                        Id = player.Id
                    }, player.Id);
                } break;
                default:
                {
                    Logging.Log(true, "Unhandled connection state changed: {0}, player: {1} ({2})", param.m_info.m_eState, player.Name, player.Id);
                } break;
            }
        }
        
        public void PollMessages()
        {
            if (!Running)
                return;
            
            var receivedMessages = new IntPtr[100];
            var incomingMessages =
                SteamNetworkingSockets.ReceiveMessagesOnPollGroup(_connectionPollGroup, receivedMessages, 100);
            if (incomingMessages <= 0)
                return;

            Logging.Log(true, "Received {0} messages", incomingMessages);
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

        public void SendTo(Packet packet, ulong connectionId)
        {
            var data = packet.Serialize().Bytes;
            var buffer = SteamNetworkingUtils.AllocateMessage(data.Length);
            Marshal.Copy(data, 0, buffer, data.Length);
            SendTo(packet, buffer, (uint) data.Length, connectionId);
        }

        public void SendTo(Packet packet, IntPtr buffer, uint length, ulong connectionId)
        {
            if (!_playerConnections.TryGetValue(connectionId, out var connection))
            {
                Logging.Error(true, "Failed to send packet to client, connection {0} doesn't exist", connectionId);
                return;
            }
            SendTo(packet, buffer, length, connection);
        }

        public void SendTo(Packet packet, HSteamNetConnection connection)
        {
            var data = packet.Serialize().Bytes;
            var buffer = SteamNetworkingUtils.AllocateMessage(data.Length);
            Marshal.Copy(data, 0, buffer, data.Length);
            SendTo(packet, buffer, (uint) data.Length, connection);
        }

        public void SendTo(Packet packet, IntPtr buffer, uint length, HSteamNetConnection connection)
        {
            if (connection.IsLocalClient())
            {
                Logging.Log(false, "Trying to send packet to local client, forwarding through flow rather than network");
                NetworkController.Client.PacketHandler.Process(packet);
                return;
            }
            
            var res = SteamNetworkingSockets.SendMessageToConnection(connection, buffer, length, 0, out _);
            if (res != EResult.k_EResultOK)
                Logging.Log(true, "Failed to send packet to client: {0}", res);
        }

        public void SendToAll(Packet packet, ulong? exclude = null)
        {
            var data = packet.Serialize().Bytes;
            var buffer = SteamNetworkingUtils.AllocateMessage(data.Length);
            Marshal.Copy(data, 0, buffer, data.Length);
            foreach (var entry in _playerConnections.Where((entry) => entry.Key != exclude))
                SendTo(packet, buffer, (uint) data.Length, entry.Value);
            // NetworkController.Client.PacketHandler.Process(packet);
        }
        
        #endregion
        
        #region Packet Listeners

        [PacketListener(PacketTypeId.RetrievePlayerList, PacketDirection.Server)]
        public void OnRetrievePlayerList(PacketRetrievePlayerList packet)
        {
            if (packet.Connection == null)
            {
                Logging.Error(true, "Failed to get connection from packet {0}, this malfunctioned packet shouldn't exist", packet.GetPacketType().Id);
                return;
            }
            
            var playerList = _playerConnections.Keys.Select((id) => new Player(id, SteamFriends.GetFriendPersonaName(id.ToSteamID()))).ToArray();
            Logging.Log(true, "Got packet: RetrievePlayerList, sending player list ({0}) to client", playerList.Length);
            SendTo(new PacketPlayerList
            {
                Players = playerList
            }, packet.Connection.Value);
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Server)]
        public void OnPacketGameStart(PacketGameStart packet)
        {
            LeaveLobby();
            SendToAll(packet);
        }
        
        [PacketListener(PacketTypeId.PlayerScore, PacketDirection.Server)]
        public void OnPacketPlayerScore(PacketPlayerScore packet)
        {
            SendToAll(packet);
        }

        [PacketListener(PacketTypeId.ShapeCreate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeMove, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeRotate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeLock, PacketDirection.Server)]
        [PacketListener(PacketTypeId.BlockBulkMove, PacketDirection.Server)]
        [PacketListener(PacketTypeId.BlockBulkRemove, PacketDirection.Server)]
        public void OnPacketForwardExclude(Packet packet)
        {
            SendToAll(packet, packet.SenderId);
        }

        [PacketListener(PacketTypeId.PlayerDead, PacketDirection.Server)]
        public void OnPlayerDead(PacketPlayerDead packet)
        {
            SendToAll(packet, packet.SenderId);
            
            if (_playerConnections.Any((entry) => _world.Containers.TryGetValue(entry.Key, out var deadContainer) && !deadContainer.dead))
                return;

            ulong? winner = null;
            var score = -1;
            var scores = new Dictionary<ulong, PlayerScore>();

            foreach (var entry in _playerConnections)
            {
                if (!_world.Containers.TryGetValue(entry.Key, out var container))
                    continue;
                
                scores.Add(container.id, container.Score);

                if (container.Score.Score <= score)
                    continue;
                
                score = container.Score.Score;
                winner = container.id;
            }
            
            SendToAll(new PacketGameEnd
            {
                Winner = winner ?? 0,
                Scores = scores
            });
        }
        
        #endregion
    }
}