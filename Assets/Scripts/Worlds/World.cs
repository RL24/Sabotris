using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sabotris.Audio;
using Sabotris.Game;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Bot;
using Sabotris.Network.Packets.Game;
using Sabotris.Network.Packets.Players;
using Sabotris.Network.Packets.Spectator;
using Sabotris.UI;
using Sabotris.UI.Menu;
using Sabotris.UI.Menu.Menus;
using Sabotris.Util;
using Steamworks;
using UnityEngine;

namespace Sabotris.Worlds
{
    public class World : MonoBehaviour
    {
        private const float PositionUpdateDelay = 200;
        
        public Container containerTemplate;
        public BotContainer botContainerTemplate;

        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public AudioController audioController;

        public SelectorOverlay selectorOverlay;
        public DemoContainer demoContainer;
        public Menu menuMain, menuPause, menuGameOver;

        public Spectator spectatorPrefab;

        public readonly List<Container> Containers = new List<Container>();
        private readonly Dictionary<Guid, Spectator> _spectators = new Dictionary<Guid, Spectator>();

        private readonly Stopwatch _positionUpdateTimer = new Stopwatch();

        private void Start()
        {
            if (networkController.Client != null)
            {
                networkController.Client.OnConnectedToServerEvent += ConnectedToServerEvent;
                networkController.Client.OnDisconnectedFromServerEvent += DisconnectedFromServerEvent;
            }

            networkController.Client?.RegisterListener(this);
            
            _positionUpdateTimer.Start();
        }

        private void Update()
        {
            if (InputUtil.ShouldPause() && !menuController.IsInMenu && networkController.Client is {IsConnected: true})
                menuController.OpenMenu(menuPause);

            if (networkController.Server?.Running == true && _positionUpdateTimer.ElapsedMilliseconds > PositionUpdateDelay)
            {
                _positionUpdateTimer.Restart();
                networkController.Client?.SendPacket(new PacketPlayerPositions
                {
                    Positions = Containers.Select((container, index) => (container.Id, GetContainerPosition(index))).ToArray()
                });
            }
        }

        private void ConnectedToServerEvent(object sender, HSteamNetConnection? connection)
        {
            if (connection == null)
                return;

            demoContainer.gameObject.SetActive(false);
            gameController.ControllingContainer = CreateContainer(Client.UserId, Client.Username, Client.SteamId.m_SteamID);
        }

        private void DisconnectedFromServerEvent(object sender, DisconnectReason disconnectReason)
        {
            gameController.ControllingContainer = null;
            foreach (var container in Containers.ToArray())
                RemoveContainer(container);
            demoContainer.gameObject.SetActive(true);
            Containers.Clear();

            menuController.OpenMenu(menuMain);

            foreach (var spectator in _spectators.Values)
                Destroy(spectator.gameObject);
            _spectators.Clear();
        }

        private Container CreateContainer(Guid id, string playerName, ulong? steamId = null)
        {
            var existingContainer = Containers.Find((c) => c.Id == id);
            if (existingContainer)
                return existingContainer;

            var container = Instantiate(containerTemplate, GetContainerPosition(Containers.Count), Quaternion.identity);
            container.name = $"Container-{playerName}-{id}";

            container.Id = id;
            container.ContainerName = playerName;
            if (steamId != null)
                container.steamId = steamId.Value;

            container.world = this;
            container.gameController = gameController;
            container.menuController = menuController;
            container.networkController = networkController;
            container.cameraController = cameraController;
            container.audioController = audioController;
            container.selectorOverlay = selectorOverlay;

            container.rawPosition = container.transform.position;

            container.transform.SetParent(transform, false);

            Containers.Add(container);

            return container;
        }

        private void RemoveContainer(Guid id)
        {
            var containers = Containers.FindAll((c) => c.Id == id);
            foreach (var container in containers)
                RemoveContainer(container);
        }

        private void RemoveContainer(Container container)
        {
            Destroy(container.gameObject);
            Containers.Remove(container);
            
            DestroySpectator(container.Id);

            var i = 0;
            foreach (var c in Containers)
                c.rawPosition = GetContainerPosition(i++);
        }

        private void CreateBot(Guid id, string botName)
        {
            var container = Instantiate(networkController.Server?.Running == true ? botContainerTemplate : containerTemplate, GetContainerPosition(Containers.Count), Quaternion.identity);
            container.name = $"Container-Bot-{botName}";

            container.Id = id;
            container.ContainerName = botName;

            container.world = this;
            container.gameController = gameController;
            container.menuController = menuController;
            container.networkController = networkController;
            container.cameraController = cameraController;

            container.rawPosition = container.transform.position;

            container.transform.SetParent(transform, false);

            Containers.Add(container);
        }

        private void CreateSpectator(Guid id, Vector3 position, Quaternion rotation)
        {
            if (_spectators.ContainsKey(id))
                DestroySpectator(id);
            
            var spectator = Instantiate(spectatorPrefab, position, rotation);
            spectator.position = position;
            spectator.rotation = rotation;

            spectator.transform.SetParent(transform.parent, true);
            
            _spectators.Add(id, spectator);
        }

        private void MoveSpectator(Guid id, Vector3 position, Quaternion rotation)
        {
            if (!_spectators.TryGetValue(id, out var spectator))
                return;

            spectator.position = position;
            spectator.rotation = rotation;
        }

        private void DestroySpectator(Guid id)
        {
            if (!_spectators.TryGetValue(id, out var spectator))
                return;
            
            _spectators.Remove(id);
            Destroy(spectator.gameObject);
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnGameStart(PacketGameStart packet)
        {
            if (gameController.ControllingContainer)
                gameController.ControllingContainer.StartDropping();

            foreach (var bot in Containers.FindAll((container) => container is BotContainer))
                bot.StartDropping();
        }

        [PacketListener(PacketTypeId.GameEnd, PacketDirection.Client)]
        public void OnGameEnd(PacketGameEnd packet)
        {
            audioController.gameOver.PlayModifiedSound(AudioController.GetGameVolume());

            audioController.music.pitch = 1;

            menuController.OpenMenu(menuGameOver);
        }

        [PacketListener(PacketTypeId.PlayerConnected, PacketDirection.Client)]
        public void OnPlayerConnected(PacketPlayerConnected packet)
        {
            if (packet.Player.Id == Client.UserId)
                return;

            CreateContainer(packet.Player.Id, packet.Player.Name);

            audioController.playerJoinLobby.PlayModifiedSound(AudioController.GetGameVolume(), AudioController.GetPlayerJoinLeavePitch());
        }

        [PacketListener(PacketTypeId.PlayerList, PacketDirection.Client)]
        public void OnPlayerList(PacketPlayerList packet)
        {
            foreach (var player in packet.Players)
            {
                if (player.Id == Client.UserId)
                    continue;

                CreateContainer(player.Id, player.Name);
            }

            foreach (var bot in packet.Bots)
                CreateBot(bot.Id, bot.Name);
        }

        [PacketListener(PacketTypeId.PlayerDisconnected, PacketDirection.Client)]
        public void OnPlayerDisconnected(PacketPlayerDisconnected packet)
        {
            RemoveContainer(packet.Id);
            audioController.playerLeaveLobby.PlayModifiedSound(AudioController.GetGameVolume(), AudioController.GetPlayerJoinLeavePitch());
        }

        [PacketListener(PacketTypeId.PlayerPositions, PacketDirection.Client)]
        public void OnPlayerDisconnected(PacketPlayerPositions packet)
        {
            foreach (var container in Containers)
            {
                var (id, position) = packet.Positions.DefaultIfEmpty((Guid.Empty, Vector3.zero)).FirstOrDefault((entry) => entry.Item1 == container.Id);
                if (id != Guid.Empty)
                    container.rawPosition = position;
            }
        }

        [PacketListener(PacketTypeId.BotConnected, PacketDirection.Client)]
        public void OnBotConnected(PacketBotConnected packet)
        {
            CreateBot(packet.Bot.Id, packet.Bot.Name);
        }

        [PacketListener(PacketTypeId.BotDisconnected, PacketDirection.Client)]
        public void OnBotDisconnected(PacketBotDisconnected packet)
        {
            RemoveContainer(packet.BotId);
        }

        [PacketListener(PacketTypeId.SpectatorCreate, PacketDirection.Client)]
        public void OnSpectatorCreate(PacketSpectatorCreate packet)
        {
            CreateSpectator(packet.Id, packet.Position, packet.Rotation);
        }

        [PacketListener(PacketTypeId.SpectatorMove, PacketDirection.Client)]
        public void OnSpectatorMove(PacketSpectatorMove packet)
        {
            MoveSpectator(packet.Id, packet.Position, packet.Rotation);
        }
        
        [PacketListener(PacketTypeId.SpectatorRemove, PacketDirection.Client)]
        public void OnSpectatorRemove(PacketSpectatorRemove packet)
        {
            DestroySpectator(packet.Id);
        }

        private Vector3 GetContainerPosition(int index) => Vector3.right * (index * ((networkController.Client?.LobbyData?.PlayFieldSize ?? 5) * 2 + 4));
    }
}