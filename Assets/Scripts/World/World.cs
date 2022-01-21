using System.Collections.Generic;
using System.Linq;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sabotris
{
    public class World : MonoBehaviour, ISerializationCallbackReceiver
    {
        public Container containerTemplate;
        
        public GameController gameController;
        public NetworkController networkController;
        public CameraController cameraController;

        private readonly Dictionary<long, Container> _containers = new Dictionary<long, Container>();

        private void Start()
        {
            networkController.Client.OnConnected += OnConnected;
            networkController.Client.OnDisconnected += OnDisconnected;
            
            networkController.Client.RegisterListener(this);
        }

        private void Update()
        {
            if (InputUtil.WasPressed(Keyboard.current.fKey))
                networkController.Client.SendPacket(new PacketGameStart());
        }

        private void OnConnected(object sender, string reason)
        {
            gameController.ControllingContainer = CreateContainer(networkController.Client.GetId(), UserUtil.GenerateUsername());
        }

        private void OnDisconnected(object sender, string reason)
        {
            gameController.ControllingContainer = null;
            foreach (var id in _containers.Keys.ToArray())
                RemoveContainer(id);
        }

        public Container CreateContainer(long id, string playerName)
        {
            if (_containers.ContainsKey(id))
                return _containers[id];
            
            var container = Instantiate(containerTemplate, _containers.Count * (Vector3.right * 8), Quaternion.identity);
            container.name = $"Container_{playerName}_{id}";

            container.id = id;
            container.containerName = playerName;
            
            container.gameController = gameController;
            container.networkController = networkController;
            container.cameraController = cameraController;
            
            container.transform.SetParent(transform, false);
            
            _containers.Add(id, container);
            
            return container;
        }

        public void RemoveContainer(long id)
        {
            if (_containers.TryGetValue(id, out var container))
                Destroy(container.gameObject);
            _containers.Remove(id);
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnGameStart(PacketGameStart packet)
        {
            if (gameController.ControllingContainer != null)
                gameController.ControllingContainer.StartDropping();
        }

        [PacketListener(PacketTypeId.PlayerConnected, PacketDirection.Client)]
        public void OnPlayerConnected(PacketPlayerConnected packet)
        {
            if (packet.Player.Id == networkController.Client.GetId())
                return;

            CreateContainer(packet.Player.Id, packet.Player.Name);
        }

        [PacketListener(PacketTypeId.PlayerList, PacketDirection.Client)]
        public void OnPlayerList(PacketPlayerList packet)
        {
            foreach (var player in packet.Players)
            {
                if (player.Id == networkController.Client.GetId())
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
            serializedContainers = _containers.Values.ToArray();
        }

        public void OnAfterDeserialize()
        {
        }
        
        #endregion
    }
}
