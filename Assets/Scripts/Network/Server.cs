﻿using System;
using System.Collections;
using System.Linq;
using Lidgren.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Network
{
    public class Server : Networker<NetServer>
    {
        public event EventHandler OnServerStart;
        public event EventHandler<string> OnServerStop;
        
        private string _password;

        public Server() : base(PacketDirection.Server)
        {
            PacketHandler.Register(this);
        }

        public IEnumerator StartServer(string password, int port)
        {
            if (Running)
            {
                Logging.Log(true, "Server already running, restarting");
                Shutdown(Reasons.RestartServer);
                yield return new WaitUntil(() => Peer.Status == NetPeerStatus.NotRunning);
            }

            Running = true;
            
            _password = password;
            
            var config = new NetPeerConfiguration(GameController.AppIdentifier)
            {
                MaximumConnections = 4,
                Port = port
            };
            Logging.Log(true, "Starting server on port {0} with password '{1}'", port, password);
            Peer = new NetServer(config);
            Peer.Start();
            
            OnServerStart?.Invoke(this, null);
        }

        public void Update()
        {
            NetIncomingMessage incoming;
            while ((incoming = Peer?.ReadMessage()) != null)
            {
                var connection = incoming.SenderConnection;
                var connectionId = connection.RemoteUniqueIdentifier;
                switch (incoming.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        Logging.Warn(true, "Logged message: {0}", incoming.ReadString());
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        Logging.Warn(true, "Logged message: {0}", incoming.ReadString());
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        var status = (NetConnectionStatus) incoming.ReadByte();
                        var reason = incoming.ReadString();

                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                            {
                                /*
                                 * Get hail message
                                 * Check if hail is correct packet data > else disconnect
                                 * Check if password matches > else disconnect
                                 * Send player list to connected player
                                 * Send player connected packet to all players
                                 */

                                var hail = connection.RemoteHailMessage;

                                if (!(GetPacket(hail) is PacketConnectingHail packet))
                                {
                                    connection.Disconnect(Reasons.InvalidConnectPacket);
                                    break;
                                }
                                
                                if (packet.Password != _password)
                                {
                                    connection.Disconnect(Reasons.IncorrectPassword);
                                    break;
                                }

                                var player = new Player(connectionId, packet.Name);

                                Peer.SendMessage(new PacketPlayerList
                                {
                                    Players = Players.Values.ToArray()
                                }.Serialize(Peer), connection, NetDeliveryMethod.ReliableOrdered);

                                Players.Add(connectionId, player);

                                Peer.SendToAll(new PacketPlayerConnected
                                {
                                    Player = player
                                }.Serialize(Peer), NetDeliveryMethod.ReliableOrdered);
                            } break;

                            case NetConnectionStatus.Disconnected:
                            {
                                if (!Players.TryGetValue(connectionId, out var player))
                                    break;

                                Players.Remove(player.Id);

                                Peer.SendToAll(new PacketPlayerDisconnected
                                {
                                    Id = connectionId
                                }.Serialize(Peer), connection, NetDeliveryMethod.ReliableOrdered, 0);

                                if (Players.Count == 0)
                                    Shutdown(Reasons.NoPlayersLeft);
                            } break;
                            
                            default:
                                Logging.Warn(true, "Status changed to {0}: {1}. For {2}", status, reason, connection.RemoteEndPoint);
                                break;
                        }
                        break;

                    case NetIncomingMessageType.Data:
                    { 
                        PacketHandler.Process(GetPacket(incoming));
                    } break;
                    
                    default:
                        Logging.Warn(true, "Unhandled message type: {0} from {1}", incoming.MessageType, incoming.SenderConnection.RemoteEndPoint);
                        break;
                }
                Peer?.Recycle(incoming);
            }
        }

        public override void Shutdown(string reason)
        {
            base.Shutdown(reason);
            OnServerStop?.Invoke(this, reason);
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Server)]
        public void OnGameStart(PacketGameStart packet)
        {
            Peer.SendToAll(packet.Serialize(Peer), NetDeliveryMethod.ReliableOrdered);
        }

        [PacketListener(PacketTypeId.ShapeCreate, PacketDirection.Server)]
        public void OnShapeCreate(PacketShapeCreate packet)
        {
            Peer.SendToAll(packet.Serialize(Peer), Peer.Connections.First((connection) => connection.RemoteUniqueIdentifier == packet.SenderId), NetDeliveryMethod.ReliableOrdered, 0);
        }

        [PacketListener(PacketTypeId.ShapeMove, PacketDirection.Server)]
        public void OnShapeMove(PacketShapeMove packet)
        {
            Peer.SendToAll(packet.Serialize(Peer), Peer.Connections.First((connection) => connection.RemoteUniqueIdentifier == packet.SenderId), NetDeliveryMethod.ReliableOrdered, 0);
        }

        [PacketListener(PacketTypeId.ShapeRotate, PacketDirection.Server)]
        public void OnShapeRotate(PacketShapeRotate packet)
        {
            Peer.SendToAll(packet.Serialize(Peer), Peer.Connections.First((connection) => connection.RemoteUniqueIdentifier == packet.SenderId), NetDeliveryMethod.ReliableOrdered, 0);
        }

        [PacketListener(PacketTypeId.ShapeLock, PacketDirection.Server)]
        public void OnShapeLock(PacketShapeLock packet)
        {
            Peer.SendToAll(packet.Serialize(Peer), Peer.Connections.First((connection) => connection.RemoteUniqueIdentifier == packet.SenderId), NetDeliveryMethod.ReliableOrdered, 0);
        }

        [PacketListener(PacketTypeId.BlockBulkMove, PacketDirection.Server)]
        public void OnBlockBulkMove(PacketBlockBulkMove packet)
        {
            Peer.SendToAll(packet.Serialize(Peer), Peer.Connections.First((connection) => connection.RemoteUniqueIdentifier == packet.SenderId), NetDeliveryMethod.ReliableOrdered, 0);
        }

        [PacketListener(PacketTypeId.BlockBulkRemove, PacketDirection.Server)]
        public void OnBlockBulkRemove(PacketBlockBulkRemove packet)
        {
            Peer.SendToAll(packet.Serialize(Peer), Peer.Connections.First((connection) => connection.RemoteUniqueIdentifier == packet.SenderId), NetDeliveryMethod.ReliableOrdered, 0);
        }

        [PacketListener(PacketTypeId.PlayerDead, PacketDirection.Server)]
        public void OnPlayerDead(PacketPlayerDead packet)
        {
            Peer.SendToAll(packet.Serialize(Peer), Peer.Connections.First((connection) => connection.RemoteUniqueIdentifier == packet.SenderId), NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}