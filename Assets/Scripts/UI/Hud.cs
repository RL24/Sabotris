﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.UI.Menu;
using Sabotris.UI.Menu.Menus;
using Sabotris.Util;
using TMPro;
using UnityEngine;

namespace Sabotris.UI
{
    public class Hud : MonoBehaviour
    {
        public NetworkController networkController;
        public MenuController menuController;

        public CanvasGroup canvasGroup;
        public GameObject playerList, scoreList;
        public TMP_Text playerItemPrefab, scoreItemPrefab;

        private readonly Dictionary<Guid, (TMP_Text, TMP_Text)> _playerScoreCache = new Dictionary<Guid, (TMP_Text, TMP_Text)>();

        private void Start()
        {
            for (var i = 0; i < playerList.transform.childCount; i++)
                Destroy(playerList.transform.GetChild(i));
            for (var i = 0; i < scoreList.transform.childCount; i++)
                Destroy(scoreList.transform.GetChild(i));

            networkController.Client?.RegisterListener(this);
            if (networkController.Client != null)
                networkController.Client.OnDisconnectedFromServerEvent += DisconnectedFromServerEvent;

            canvasGroup.alpha = 0;
        }

        private void OnDestroy()
        {
            networkController.Client?.DeregisterListener(this);
        }

        private void Update()
        {
            canvasGroup.alpha += canvasGroup.alpha.Lerp((_playerScoreCache.Any() && (!menuController.IsInMenu || menuController.currentMenu is MenuGameOver)).Int(), GameSettings.Settings.uiAnimationSpeed);
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

            foreach (var player in packet.Players)
                AddEntry(player);

            foreach (var bot in packet.Bots)
                AddEntry(bot);
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