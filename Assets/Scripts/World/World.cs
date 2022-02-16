using System.Collections.Generic;
using System.Linq;
using Network;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using UI.Menu;
using UI.Menu.Menus;
using UnityEngine;

namespace Sabotris
{
    public class World : MonoBehaviour, ISerializationCallbackReceiver
    {
        public Container containerTemplate;
        
        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;

        public DemoContainer demoContainer;
        public Menu menuMain, menuPause, menuGameOver;

        public readonly Dictionary<ulong, Container> Containers = new Dictionary<ulong, Container>();

        private void Start()
        {
            networkController.Client.OnConnectedToServerEvent += ConnectedToServerEvent;
            networkController.Client.OnDisconnectedFromServerEvent += DisconnectedFromServerEvent;
            
            networkController.Client.RegisterListener(this);
        }

        private void Update()
        {
            if (InputUtil.ShouldPause() && !menuController.IsInMenu && networkController.Client is {IsConnected: true})
                menuController.OpenMenu(menuPause);
        }

        private void ConnectedToServerEvent(object sender, bool success)
        {
            if (!success)
                return;
            
            demoContainer.gameObject.SetActive(false);
            gameController.ControllingContainer =
                CreateContainer(Client.UserId, Client.Username);
        }

        private void DisconnectedFromServerEvent(object sender, DisconnectReason disconnectReason)
        {
            gameController.ControllingContainer = null;
            foreach (var id in Containers.Keys.ToArray())
                RemoveContainer(id);
            demoContainer.gameObject.SetActive(true);
            
            menuController.OpenMenu(menuMain);
        }

        public Container CreateContainer(ulong id, string playerName)
        {
            if (Containers.ContainsKey(id))
                return Containers[id];
            
            var container = Instantiate(containerTemplate, Containers.Count * (Vector3.right * (Container.Radius * 2 + 4)), Quaternion.identity);
            container.name = $"Container_{playerName}_{id}";

            container.id = id;
            container.ContainerName = playerName;
            
            container.gameController = gameController;
            container.menuController = menuController;
            container.networkController = networkController;
            container.cameraController = cameraController;
            
            container.transform.SetParent(transform, false);
            
            Containers.Add(id, container);
            
            return container;
        }

        public void RemoveContainer(ulong id)
        {
            if (Containers.TryGetValue(id, out var container))
                Destroy(container.gameObject);
            Containers.Remove(id);
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnGameStart(PacketGameStart packet)
        {
            if (gameController.ControllingContainer != null)
                gameController.ControllingContainer.StartDropping();
        }

        [PacketListener(PacketTypeId.GameEnd, PacketDirection.Client)]
        public void OnGameEnd(PacketGameEnd packet)
        {
            menuController.OpenMenu(menuGameOver);
        }

        [PacketListener(PacketTypeId.PlayerConnected, PacketDirection.Client)]
        public void OnPlayerConnected(PacketPlayerConnected packet)
        {
            if (packet.Player.Id == Client.UserId)
                return;

            CreateContainer(packet.Player.Id, packet.Player.Name);
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
        }

        [PacketListener(PacketTypeId.PlayerDisconnected, PacketDirection.Client)]
        public void OnPlayerDisconnected(PacketPlayerDisconnected packet)
        {
            RemoveContainer(packet.Id);
        }
        
        #region Serialize Fields
        
        public Container[] serializedContainers;
        
        public void OnBeforeSerialize()
        {
            serializedContainers = Containers.Values.ToArray();
        }

        public void OnAfterDeserialize()
        {
        }
        
        #endregion
    }
}
