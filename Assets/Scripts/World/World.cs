using System;
using System.Collections.Generic;
using Sabotris.Audio;
using Sabotris.Game;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Bot;
using Sabotris.Network.Packets.Game;
using Sabotris.UI.Menu;
using Sabotris.UI.Menu.Menus;
using Sabotris.Util;
using Steamworks;
using UnityEngine;

namespace Sabotris
{
    public class World : MonoBehaviour
    {
        public Container containerTemplate;
        public BotContainer botContainerTemplate;

        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public AudioController audioController;

        public DemoContainer demoContainer;
        public Menu menuMain, menuPause, menuGameOver;

        public readonly List<Container> Containers = new List<Container>();

        private void Start()
        {
            if (networkController.Client != null)
            {
                networkController.Client.OnConnectedToServerEvent += ConnectedToServerEvent;
                networkController.Client.OnDisconnectedFromServerEvent += DisconnectedFromServerEvent;
            }

            networkController.Client?.RegisterListener(this);
        }

        private void Update()
        {
            if (InputUtil.ShouldPause() && !menuController.IsInMenu && networkController.Client is {IsConnected: true})
                menuController.OpenMenu(menuPause);
        }

        private void ConnectedToServerEvent(object sender, HSteamNetConnection? connection)
        {
            if (connection == null)
                return;

            demoContainer.gameObject.SetActive(false);
            gameController.ControllingContainer =
                CreateContainer(Client.UserId, Client.Username, Client.SteamId.m_SteamID);
        }

        private void DisconnectedFromServerEvent(object sender, DisconnectReason disconnectReason)
        {
            gameController.ControllingContainer = null;
            foreach (var container in Containers.ToArray())
                RemoveContainer(container);
            demoContainer.gameObject.SetActive(true);

            menuController.OpenMenu(menuMain);
        }

        private Container CreateContainer(Guid id, string playerName, ulong? steamId = null)
        {
            var existingContainer = Containers.Find((c) => c.id == id);
            if (existingContainer)
                return existingContainer;

            var container = Instantiate(containerTemplate, GetContainerPosition(Containers.Count), Quaternion.identity);
            container.name = $"Container-{playerName}-{id}";

            container.id = id;
            container.ContainerName = playerName;

            container.world = this;
            container.gameController = gameController;
            container.menuController = menuController;
            container.networkController = networkController;
            container.cameraController = cameraController;
            container.audioController = audioController;

            container.rawPosition = container.transform.position;

            container.transform.SetParent(transform, false);

            Containers.Add(container);

            return container;
        }

        private void RemoveContainer(Guid id)
        {
            var containers = Containers.FindAll((c) => c.id == id);
            foreach (var container in containers)
                RemoveContainer(container);
        }

        private void RemoveContainer(Container container)
        {
            Destroy(container.gameObject);
            Containers.Remove(container);

            var i = 0;
            foreach (var c in Containers)
                c.rawPosition = GetContainerPosition(i++);
        }

        private void CreateBot(Guid id, string botName)
        {
            var container = Instantiate(networkController.Server?.Running == true ? botContainerTemplate : containerTemplate, GetContainerPosition(Containers.Count), Quaternion.identity);
            container.name = $"Container-Bot-{botName}";

            container.id = id;
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

        private Vector3 GetContainerPosition(int index) => Vector3.right * (index * ((networkController.Client?.LobbyData?.PlayFieldSize ?? 5) * 2 + 4));
    }
}