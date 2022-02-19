using System.Collections.Generic;
using System.Linq;
using Network;
using Sabotris;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        public NetworkController networkController;
        
        public CanvasGroup canvasGroup;
        public GameObject playerList, scoreList;
        public TMP_Text playerItemPrefab, scoreItemPrefab;

        private readonly Dictionary<ulong, (TMP_Text, TMP_Text)> _playerScoreCache = new Dictionary<ulong, (TMP_Text, TMP_Text)>();

        private void Start()
        {
            for (var i = 0; i < playerList.transform.childCount; i++)
                Destroy(playerList.transform.GetChild(i));
            for (var i = 0; i < scoreList.transform.childCount; i++)
                Destroy(scoreList.transform.GetChild(i));

            networkController.Client.RegisterListener(this);
            networkController.Client.OnDisconnectedFromServerEvent += DisconnectedFromServerEvent;

            canvasGroup.alpha = 0;
        }

        private void OnDestroy()
        {
            networkController.Client.DeregisterListener(this);
        }

        private void Update()
        {
            canvasGroup.alpha += canvasGroup.alpha.Lerp(_playerScoreCache.Any().Int(), GameSettings.UIAnimationSpeed);
        }

        private void AddEntry(Player player)
        {
            var playerItem = Instantiate(playerItemPrefab, Vector3.zero, Quaternion.identity, playerList.transform);
            playerItem.name = $"Player-{player.Name}-{player.Id}";
            playerItem.text = player.Name;

            var scoreItem = Instantiate(scoreItemPrefab, Vector3.zero, Quaternion.identity, scoreList.transform);
            scoreItem.name = $"Score-{player.Name}-{player.Id}";
            scoreItem.text = "0";
            
            _playerScoreCache.Add(player.Id, (playerItem, scoreItem));
        }

        private void RemoveEntry((TMP_Text, TMP_Text) entry)
        {
            Destroy(entry.Item1.gameObject);
            Destroy(entry.Item2.gameObject);
        }

        private void RemoveAllEntries()
        {
            foreach (var entry in _playerScoreCache.Values)
                RemoveEntry(entry);

            _playerScoreCache.Clear();
        }

        private void DisconnectedFromServerEvent(object sender, DisconnectReason disconnectReason)
        {
            RemoveAllEntries();
        }

        [PacketListener(PacketTypeId.PlayerConnected, PacketDirection.Client)]
        public void OnPlayerConnected(PacketPlayerConnected packet)
        {
            AddEntry(packet.Player);
        }

        [PacketListener(PacketTypeId.PlayerDisconnected, PacketDirection.Client)]
        public void OnPlayerDisconnected(PacketPlayerDisconnected packet)
        {
            if (!_playerScoreCache.TryGetValue(packet.Id, out var item))
                return;
            
            RemoveEntry(item);

            _playerScoreCache.Remove(packet.Id);
        }

        [PacketListener(PacketTypeId.PlayerList, PacketDirection.Client)]
        public void OnPlayerList(PacketPlayerList packet)
        {
            RemoveAllEntries();
            
            foreach (var packetPlayer in packet.Players)
                AddEntry(packetPlayer);
        }

        [PacketListener(PacketTypeId.PlayerScore, PacketDirection.Client)]
        public void OnPlayerScore(PacketPlayerScore packet)
        {
            if (!_playerScoreCache.TryGetValue(packet.Id, out var item))
                return;

            item.Item2.text = $"{packet.Score.Score}";
        }
    }
}