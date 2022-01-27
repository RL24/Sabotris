using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Lidgren.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Sabotris.Network
{
    public class Client : Networker<NetClient>
    {
        private const float ConnectTimeoutMs = 5000;
        
        public event EventHandler<string> OnConnected;
        public event EventHandler<string> OnDisconnected;

        public string UserName;
        
        public Client() : base(PacketDirection.Client) {}
        
        public IEnumerator StartClient(string ip, int port, string password)
        {
            if (Running)
            {
                Logging.Log(false, "Client already running, restarting");
                Shutdown(Reasons.RestartClient);
                yield return new WaitUntil(() => Peer.Status == NetPeerStatus.NotRunning);
            }

            Running = true;

            var config = new NetPeerConfiguration(GameController.AppIdentifier)
            {
                AutoFlushSendQueue = false,
                ConnectionTimeout = 5000
            };
            Logging.Log(false, "Starting client, connecting to server {0}:{1} with password '{2}'", ip, port, password);
            Peer = new NetClient(config);
            Peer.Start();
            
            Peer.RegisterReceivedCallback(Update);

            var failed = new[] {false};
            new Thread(() =>
            {
                try
                {
                    Peer.Connect(ip, port, new PacketConnectingHail
                    {
                        Password = password,
                        Name = UserName = UserUtil.GenerateUsername()
                    }.Serialize(Peer));
                }
                catch (NetException)
                {
                    failed[0] = true;
                }
            }).Start();

            var timeout = new Stopwatch();
            timeout.Start();
            yield return new WaitUntil(() => IsConnected || timeout.ElapsedMilliseconds > ConnectTimeoutMs || failed[0]);
            timeout.Reset();
            if (!IsConnected || failed[0])
                Shutdown(Reasons.ServerNotFound);
        }
        
        public void Update(object peer)
        {
            NetIncomingMessage incoming;
            while ((incoming = Peer?.ReadMessage()) != null)
            {
                var connection = incoming.SenderConnection;
                switch (incoming.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        Logging.Warn(false, "Logged message: {0}", incoming.ReadString());
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        Logging.Error(false, "Logged error: {0}", incoming.ReadString());
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        var status = (NetConnectionStatus) incoming.ReadByte();
                        var reason = incoming.ReadString();

                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                            {
                                OnConnected?.Invoke(this, reason);
                            } break;

                            case NetConnectionStatus.Disconnected:
                            {
                                OnDisconnected?.Invoke(this, reason);
                                Peer.UnregisterReceivedCallback(Update);
                                Peer = null;
                                Players.Clear();
                                Running = false;
                            } break;
                            
                            default:
                                Logging.Warn(false, "Status changed: {0}: {1}", status, reason);
                                break;
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        PacketHandler.Process(GetPacket(incoming));
                        break;
                    
                    default:
                        Logging.Warn(false, "Unhandled message type: {0}", incoming.MessageType);
                        break;
                }
                Peer?.Recycle(incoming);
            }
        }

        public override void Shutdown(string reason)
        {
            Peer?.UnregisterReceivedCallback(Update);
            Peer?.Disconnect(reason);
            base.Shutdown(reason);
            OnDisconnected?.Invoke(this, reason);
        }

        public void SendPacket(Packet packet, bool flush = true)
        {
            Peer?.SendMessage(packet.Serialize(Peer), NetDeliveryMethod.ReliableOrdered);
            if (flush)
                Peer?.FlushSendQueue();
        }

        public long UserId => Peer?.UniqueIdentifier ?? -1;

        public bool IsConnected => Peer?.ConnectionStatus == NetConnectionStatus.Connected;
    }
}