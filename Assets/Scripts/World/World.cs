using System;
using System.Collections.Generic;
using Audio;
using Sabotris.Network;
using Sabotris.Network.Packets;
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
                CreateContainer(Client.UserId.m_SteamID, Client.Username);
        }

        private void DisconnectedFromServerEvent(object sender, DisconnectReason disconnectReason)
        {
            gameController.ControllingContainer = null;
            foreach (var container in Containers.ToArray())
                RemoveContainer(container);
            demoContainer.gameObject.SetActive(true);

            menuController.OpenMenu(menuMain);
        }

        private Container CreateContainer(ulong id, string playerName)
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

        private void RemoveContainer(ulong id)
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

        private void CreateBot()
        {
            var container = Instantiate(botContainerTemplate, GetContainerPosition(Containers.Count), Quaternion.identity);
            container.name = $"Container-Bot";

            container.ContainerName = "Bot";

            container.world = this;
            container.gameController = gameController;
            container.menuController = menuController;
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
            if (packet.Player.Id == Client.UserId.m_SteamID)
                return;

            CreateContainer(packet.Player.Id, packet.Player.Name);
            
            audioController.playerJoinLobby.PlayModifiedSound(AudioController.GetGameVolume(), AudioController.GetPlayerJoinLeavePitch());
        }

        [PacketListener(PacketTypeId.PlayerList, PacketDirection.Client)]
        public void OnPlayerList(PacketPlayerList packet)
        {
            foreach (var player in packet.Players)
            {
                if (player.Id == Client.UserId.m_SteamID)
                    continue;

                CreateContainer(player.Id, player.Name);
            }
        }

        [PacketListener(PacketTypeId.PlayerDisconnected, PacketDirection.Client)]
        public void OnPlayerDisconnected(PacketPlayerDisconnected packet)
        {
            RemoveContainer(packet.Id);
            audioController.playerLeaveLobby.PlayModifiedSound(AudioController.GetGameVolume(), AudioController.GetPlayerJoinLeavePitch());
        }

        private Vector3 GetContainerPosition(int index) => Vector3.right * (index * ((networkController.Client?.LobbyData?.PlayFieldSize ?? 5) * 2 + 4));
    }
}