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
        public event EventHandler OnServerStartEvent;
        public event EventHandler<DisconnectReason> OnServerStopEvent;
        
        private readonly World _world;
        
        private readonly Dictionary<ulong, HSteamNetConnection> _playerConnections = new Dictionary<ulong, HSteamNetConnection>();
        private HSteamNetPollGroup _connectionPollGroup;

        public CSteamID? LobbyId;
        private HSteamListenSocket? _listenSocket;

        private string _lobbyName;
        private int _lobbyPlayerCount;

        public bool Starting;
        public bool Running => _listenSocket != null;

        public Server(NetworkController networkController, World world) : base(networkController, PacketDirection.Server)
        {
            _world = world;
            
            PacketHandler.Register(this);

            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            // Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdated);
            Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionStatusChanged);

            OnServerStartEvent += OnServerStart;
            OnServerStopEvent += OnServerStop;
        }

        private void OnServerStart(object sender, EventArgs e)
        {
            Starting = false;
        }

        private void OnServerStop(object sender, DisconnectReason e)
        {
            LeaveLobby();
            Starting = false;
        }

        public void CreateLobby(string lobbyName)
        {
            Starting = true;
            _lobbyName = lobbyName;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
        }

        private void OnLobbyCreated(LobbyCreated_t param)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                Logging.Log(true, "Failed to create lobby: {0}", param.m_eResult);
                return;
            }

            LobbyId = param.m_ulSteamIDLobby.ToSteamID();
            
            SteamMatchmaking.SetLobbyData(LobbyId.Value, HostIdKey, Client.UserId.m_SteamID.ToString());
            SteamMatchmaking.SetLobbyData(LobbyId.Value, LobbyNameKey, _lobbyName);
            // todo: set user count in lobby data

            CreateListenerSocket();
        }
        
        public void CreateListenerSocket()
        {
            _connectionPollGroup = SteamNetworkingSockets.CreatePollGroup();
            _listenSocket = SteamNetworkingSockets.CreateListenSocketP2P(0, 0, new SteamNetworkingConfigValue_t[0]);
            OnServerStartEvent?.Invoke(this, null);
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
                    Logging.Log(true, "Player connected: {0}", player.Name);
                    _playerConnections.Add(player.Id, param.m_hConn);
                    SteamNetworkingSockets.SetConnectionPollGroup(param.m_hConn, _connectionPollGroup);

                    SendPacketToAll(new PacketPlayerConnected
                    {
                        Player = player
                    }, player.Id);
                    break;
                
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
                    SteamNetworkingSockets.AcceptConnection(param.m_hConn);
                    break;
                
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
                    SteamNetworkingSockets.CloseConnection(param.m_hConn, 0, "", false);
                    _playerConnections.Remove(player.Id);
                    SendPacketToAll(new PacketPlayerDisconnected
                    {
                        Id = player.Id
                    }, player.Id);
                    break;
                
                default:
                    Logging.Log(true, "Unhandled connection state changed: {0} for player: {1} ({2})", param.m_info.m_eState, player.Name, player.Id);
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

        public void DisconnectSockets(DisconnectReason? reason = null)
        {
            SteamNetworkingSockets.DestroyPollGroup(_connectionPollGroup);
            
            foreach (var connection in _playerConnections.Values)
                SteamNetworkingSockets.CloseConnection(connection, 0, "", false);
            _playerConnections.Clear();

            if (_listenSocket != null)
                SteamNetworkingSockets.CloseListenSocket(_listenSocket.Value);
            _listenSocket = null;
            
            OnServerStopEvent?.Invoke(this, reason ?? DisconnectReason.ServerClosed);
        }

        public void PollMessages()
        {
            if (!Running)
                return;
            
            var receivedMessages = new IntPtr[100];
            var incomingMessages = SteamNetworkingSockets.ReceiveMessagesOnPollGroup(_connectionPollGroup, receivedMessages, 100);
            ProcessIncomingMessages(receivedMessages, incomingMessages);
        }

        // private void OnLobbyChatUpdated(LobbyChatUpdate_t param)
        // {
        //     var connectedSteamUser = param.m_ulSteamIDUserChanged.ToSteamID();
        //     var connectedSteamUserName = SteamFriends.GetFriendPersonaName(connectedSteamUser);
        //     Logging.Log(true, "Lobby chat updated: {0} ({1})", connectedSteamUserName, (EChatMemberStateChange) param.m_rgfChatMemberStateChange);
        // }
        
        #region Sending Packets

        public void SendPacket(Packet packet, HSteamNetConnection connection)
        {
            var data = packet.Serialize().Bytes;
            var message = Marshal.PtrToStructure<SteamNetworkingMessage_t>(SteamNetworkingUtils.AllocateMessage(data.Length));
            Marshal.Copy(data, 0, message.m_pData, data.Length);
            SendPacket(packet, message, (uint) data.Length, connection);
        }

        public void SendPacket(Packet packet, SteamNetworkingMessage_t message, uint length, HSteamNetConnection connection)
        {
            Logging.Log(true, "Sending packet: {0} ({1} bytes)", packet.GetPacketType().Id, length);
            
            if (connection.IsLocalClient())
            {
                NetworkController.Client.PacketHandler.Process(packet);
                return;
            }
            
            SendNetworkMessage(connection, message, length);
        }

        private void SendPacketToAll(Packet packet, ulong? exclude = null)
        {
            var data = packet.Serialize().Bytes;
            var buffer = Marshal.PtrToStructure<SteamNetworkingMessage_t>(SteamNetworkingUtils.AllocateMessage(data.Length));
            Logging.Log(true, "Marshalling data length: {0} for packet {1}", data.Length, packet.GetPacketType().Id);
            Marshal.Copy(data, 0, buffer.m_pData, data.Length);
            foreach (var entry in _playerConnections.Where((entry) => entry.Key != exclude))
                SendPacket(packet, buffer, (uint) data.Length, entry.Value);
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
            SendPacket(new PacketPlayerList
            {
                Players = playerList
            }, packet.Connection.Value);
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Server)]
        public void OnPacketGameStart(PacketGameStart packet)
        {
            LeaveLobby();
            SendPacketToAll(packet);
        }
        
        [PacketListener(PacketTypeId.PlayerScore, PacketDirection.Server)]
        public void OnPacketPlayerScore(PacketPlayerScore packet)
        {
            SendPacketToAll(packet);
        }

        [PacketListener(PacketTypeId.ShapeCreate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeMove, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeRotate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeLock, PacketDirection.Server)]
        [PacketListener(PacketTypeId.BlockBulkMove, PacketDirection.Server)]
        [PacketListener(PacketTypeId.BlockBulkRemove, PacketDirection.Server)]
        public void OnPacketForwardExclude(Packet packet)
        {
            SendPacketToAll(packet, packet.SenderId);
        }

        [PacketListener(PacketTypeId.PlayerDead, PacketDirection.Server)]
        public void OnPlayerDead(PacketPlayerDead packet)
        {
            SendPacketToAll(packet, packet.SenderId);
            
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
            
            SendPacketToAll(new PacketGameEnd
            {
                Winner = winner ?? 0,
                Scores = scores
            });
        }
        
        #endregion
    }
}