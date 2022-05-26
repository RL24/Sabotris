using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Sabotris.Game;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Bot;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using Sabotris.Worlds;
using Steamworks;
using Random = Sabotris.Util.Random;

namespace Sabotris.UI.Menu.Menus
{
    public class Server : Networker
    {
        public event EventHandler OnServerStartEvent;
        public event EventHandler<DisconnectReason> OnServerStopEvent;

        private readonly World _world;

        private readonly Dictionary<ulong, (Player, HSteamNetConnection)> _steamConnections = new Dictionary<ulong, (Player, HSteamNetConnection)>();
        private readonly Dictionary<Guid, Player> _bots = new Dictionary<Guid, Player>();
        private HSteamNetPollGroup _connectionPollGroup;

        public CSteamID? LobbyId;
        private LobbyData _lobbyData = new LobbyData();
        private HSteamListenSocket? _listenSocket;

        public bool Starting;
        public bool Running => _listenSocket != null;

        public Server(NetworkController networkController, World world) : base(networkController, PacketDirection.Server)
        {
            _world = world;

            PacketHandler.Register(this);

            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
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

        public void CreateLobby(LobbyData lobbyData)
        {
            Starting = true;
            _lobbyData = lobbyData;

            _bots.Clear();
            for (var i = 0; i < _lobbyData.BotCount; i++)
            {
                var player = new Player(Guid.NewGuid(), Random.RandomName(5));
                _bots.Add(player.Id, player);
            }

            SteamMatchmaking.CreateLobby(lobbyData.PracticeMode ? ELobbyType.k_ELobbyTypeInvisible : ELobbyType.k_ELobbyTypePublic, lobbyData.MaxPlayers);
        }

        private void OnLobbyCreated(LobbyCreated_t param)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                Logging.Log(true, "Failed to create lobby: {0}", param.m_eResult);
                return;
            }

            LobbyId = param.m_ulSteamIDLobby.ToSteamID();

            _lobbyData.Store(LobbyId);

            if (NetworkController.Client != null)
                NetworkController.Client.LobbyData = _lobbyData;

            CreateListenerSocket();
        }

        private void CreateListenerSocket()
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

            _steamConnections.TryGetValue(steamId.m_SteamID, out var player);
            player.Item1 ??= new Player(Guid.NewGuid(), SteamFriends.GetFriendPersonaName(steamId), steamId.m_SteamID);

            switch (param.m_info.m_eState)
            {
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
                    _steamConnections.Add(steamId.m_SteamID, (player.Item1, param.m_hConn));
                    SteamNetworkingSockets.SetConnectionPollGroup(param.m_hConn, _connectionPollGroup);

                    _lobbyData.UpdatePlayerCount(LobbyId, ConnectionCount);

                    SendPacket(new PacketRetrievePlayerId
                    {
                        SteamId = steamId.m_SteamID,
                        IsNewConnection = true
                    }, param.m_hConn);
                    break;

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
                    SteamNetworkingSockets.AcceptConnection(param.m_hConn);
                    break;

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
                    SteamNetworkingSockets.CloseConnection(param.m_hConn, 0, "", false);
                    _steamConnections.Remove(steamId.m_SteamID);

                    _lobbyData.UpdatePlayerCount(LobbyId, ConnectionCount);

                    SendPacketToAll(new PacketPlayerDisconnected
                    {
                        Id = player.Item1.Id
                    }, steamId.m_SteamID);
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

            foreach (var connection in _steamConnections.Values)
                SteamNetworkingSockets.CloseConnection(connection.Item2, 0, "", false);
            _steamConnections.Clear();

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

        #region Sending Packets

        private void SendPacket(Packet packet, HSteamNetConnection connection)
        {
            var data = packet.Serialize().Bytes;
            var message = Marshal.PtrToStructure<SteamNetworkingMessage_t>(SteamNetworkingUtils.AllocateMessage(data.Length));
            Marshal.Copy(data, 0, message.m_pData, data.Length);
            SendPacket(packet, message, (uint) data.Length, connection);
        }

        private void SendPacket(Packet packet, SteamNetworkingMessage_t message, uint length, HSteamNetConnection connection)
        {
            if (connection.IsLocalClient())
            {
                NetworkController.Client?.PacketHandler.Process(packet);
                return;
            }

            SendNetworkMessage(connection, message, length);
        }

        private void SendPacketToAll(Packet packet, ulong? exclude = null)
        {
            var data = packet.Serialize().Bytes;
            var buffer = Marshal.PtrToStructure<SteamNetworkingMessage_t>(SteamNetworkingUtils.AllocateMessage(data.Length));
            Marshal.Copy(data, 0, buffer.m_pData, data.Length);

            foreach (var entry in _steamConnections.Where((entry) => entry.Key != exclude))
                SendPacket(packet, buffer, (uint) data.Length, entry.Value.Item2);
        }

        private int ConnectionCount => _steamConnections.Count;

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

            var playerList = _steamConnections.Values.Select((entry) => entry.Item1).ToArray();
            var botList = _bots.Values.ToArray();
            SendPacket(new PacketPlayerList
            {
                Players = playerList,
                Bots = botList
            }, packet.Connection.Value);
        }

        [PacketListener(PacketTypeId.RetrievePlayerId, PacketDirection.Server)]
        public void OnRetrievePlayerId(PacketRetrievePlayerId packet)
        {
            if (packet.Connection == null)
            {
                Logging.Error(true, "Failed to get connection from packet {0}, this malfunctioned packet shouldn't exist", packet.GetPacketType().Id);
                return;
            }

            if (!_steamConnections.TryGetValue(packet.SteamId, out var player))
                return;

            player.Item1.Id = packet.Id;
            if (packet.IsNewConnection)
            {
                SendPacketToAll(new PacketPlayerConnected
                {
                    Player = player.Item1
                }, packet.SteamId);
            }
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Server)]
        public void OnPacketGameStart(PacketGameStart packet)
        {
            LeaveLobby();
            SendPacketToAll(packet);
        }

        [PacketListener(PacketTypeId.ChatMessage, PacketDirection.Server)]
        [PacketListener(PacketTypeId.PlayerScore, PacketDirection.Server)]
        public void OnPacketForward(Packet packet)
        {
            SendPacketToAll(packet);
        }

        [PacketListener(PacketTypeId.ShapeCreate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeMove, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeRotate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.ShapeLock, PacketDirection.Server)]
        [PacketListener(PacketTypeId.FallingShapeCreate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.BlockBulkCreate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.BlockBulkRemove, PacketDirection.Server)]
        [PacketListener(PacketTypeId.BlockCreate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.FallingBlockCreate, PacketDirection.Server)]
        [PacketListener(PacketTypeId.LayerMove, PacketDirection.Server)]
        [PacketListener(PacketTypeId.LayerClear, PacketDirection.Server)]
        [PacketListener(PacketTypeId.LayerAdd, PacketDirection.Server)]
        public void OnPacketForwardExclude(Packet packet)
        {
            SendPacketToAll(packet, packet.SenderId);
        }

        [PacketListener(PacketTypeId.PlayerDead, PacketDirection.Server)]
        public void OnPlayerDead(PacketPlayerDead packet)
        {
            SendPacketToAll(packet, packet.SenderId);

            if (_world.Containers.Any((entry) => !entry.dead))
                return;

            SendPacketToAll(new PacketGameEnd());
        }

        [PacketListener(PacketTypeId.BotDisconnected, PacketDirection.Server)]
        public void OnBotDisconnected(PacketBotDisconnected packet)
        {
            _bots.Remove(packet.BotId);

            SendPacketToAll(new PacketBotDisconnected
            {
                BotId = packet.BotId
            }, packet.SenderId);
        }

        #endregion
    }
}