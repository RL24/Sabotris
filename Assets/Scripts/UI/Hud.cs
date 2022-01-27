using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly Dictionary<long, Pair<TMP_Text, TMP_Text>> _playerScoreCache = new Dictionary<long, Pair<TMP_Text, TMP_Text>>();

        private void Start()
        {
            for (var i = 0; i < playerList.transform.childCount; i++)
                Destroy(playerList.transform.GetChild(i));
            for (var i = 0; i < scoreList.transform.childCount; i++)
                Destroy(scoreList.transform.GetChild(i));

            networkController.Client.RegisterListener(this);
            networkController.Client.OnDisconnected += OnDisconnectedFromServer;

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

        private void RemoveEntry(Pair<TMP_Text, TMP_Text> entry)
        {
            Destroy(entry.Key.gameObject);
            Destroy(entry.Value.gameObject);
        }

        [PacketListener(PacketTypeId.PlayerConnected, PacketDirection.Client)]
        public void OnPlayerConnected(PacketPlayerConnected packet)
        {
            var playerItem = Instantiate(playerItemPrefab, Vector3.zero, Quaternion.identity, playerList.transform);
            playerItem.name = $"Player-{packet.Player.Name}-{packet.Player.Id}";
            playerItem.text = packet.Player.Name;

            var scoreItem = Instantiate(scoreItemPrefab, Vector3.zero, Quaternion.identity, scoreList.transform);
            scoreItem.name = $"Score-{packet.Player.Name}-{packet.Player.Id}";
            scoreItem.text = "0";
            
            _playerScoreCache.Add(packet.Player.Id, new Pair<TMP_Text, TMP_Text>(playerItem, scoreItem));
        }

        private void OnDisconnectedFromServer(object sender, string reason)
        {
            foreach (var entry in _playerScoreCache.Values)
                RemoveEntry(entry);

            _playerScoreCache.Clear();
        }

        [PacketListener(PacketTypeId.PlayerDisconnected, PacketDirection.Client)]
        public void OnPlayerDisconnected(PacketPlayerDisconnected packet)
        {
            if (!_playerScoreCache.TryGetValue(packet.Id, out var item))
                return;
            
            RemoveEntry(item);

            _playerScoreCache.Remove(packet.Id);
        }

        [PacketListener(PacketTypeId.PlayerScore, PacketDirection.Client)]
        public void OnPlayerScore(PacketPlayerScore packet)
        {
            if (!_playerScoreCache.TryGetValue(packet.Id, out var item))
                return;

            item.Value.text = $"{packet.Score}";
        }
    }
}