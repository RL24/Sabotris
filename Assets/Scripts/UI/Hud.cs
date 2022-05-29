using System;
using System.Collections.Generic;
using System.Linq;
using Sabotris.Game;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Network.Packets.Players;
using Sabotris.Powers;
using Sabotris.Translations;
using Sabotris.UI.Menu;
using Sabotris.UI.Menu.Menus;
using Sabotris.Util;
using Sabotris.Worlds;
using TMPro;
using UnityEngine;

namespace Sabotris.UI
{
    public class Hud : MonoBehaviour
    {
        public GameController gameController;
        public NetworkController networkController;
        public MenuController menuController;
        
        public CanvasGroup canvasGroup;
        public GameObject playerList, scoreList, powerUpList;
        public TMP_Text playerItemPrefab, scoreItemPrefab, powerUpItemPrefab;

        private readonly Dictionary<Guid, (TMP_Text, TMP_Text)> _playerScoreCache = new Dictionary<Guid, (TMP_Text, TMP_Text)>();
        private readonly Dictionary<PowerUp, TMP_Text> _powerUpCache = new Dictionary<PowerUp, TMP_Text>();

        private Container _controllingContainer;

        private void Start()
        {
            for (var i = 0; i < playerList.transform.childCount; i++)
                Destroy(playerList.transform.GetChild(i));
            for (var i = 0; i < scoreList.transform.childCount; i++)
                Destroy(scoreList.transform.GetChild(i));
            for (var i = 0; i < powerUpList.transform.childCount; i++)
                Destroy(powerUpList.transform.GetChild(i));

            networkController.Client?.RegisterListener(this);
            if (networkController.Client != null)
                networkController.Client.OnDisconnectedFromServerEvent += DisconnectedFromServerEvent;

            canvasGroup.alpha = 0;

            Localization.LanguageChangedEvent += OnLanguageChanged;
        }

        private void OnDestroy()
        {
            Localization.LanguageChangedEvent -= OnLanguageChanged;
            networkController.Client?.DeregisterListener(this);
        }

        private void Update()
        {
            canvasGroup.alpha += canvasGroup.alpha.Lerp((_playerScoreCache.Any() && (!menuController.IsInMenu || menuController.currentMenu is MenuGameOver)).Int(), GameSettings.Settings.uiAnimationSpeed.Delta());

            if (_controllingContainer != gameController.ControllingContainer)
            {
                if (_controllingContainer)
                {
                    _controllingContainer.OnAddPowerUp -= OnAddPowerUp;
                    _controllingContainer.OnRemovePowerUp -= OnRemovePowerUp;
                }

                _controllingContainer = gameController.ControllingContainer;

                if (_controllingContainer)
                {
                    _controllingContainer.OnAddPowerUp += OnAddPowerUp;
                    _controllingContainer.OnRemovePowerUp += OnRemovePowerUp;
                }
            }
        }

        private void OnAddPowerUp(object sender, PowerUp powerUp)
        {
            AddPowerUpEntry(powerUp);
        }

        private void OnRemovePowerUp(object sender, PowerUp powerUp)
        {
            if (!_powerUpCache.TryGetValue(powerUp, out var powerUpEntry))
                return;
            _powerUpCache.Remove(powerUp);
            Destroy(powerUpEntry.gameObject);
        }

        private void AddScoreEntry(Player player)
        {
            var playerItem = Instantiate(playerItemPrefab, Vector3.zero, Quaternion.identity, playerList.transform);
            playerItem.name = $"Player-{player.Name}-{player.Id}";
            playerItem.text = player.Name;

            var scoreItem = Instantiate(scoreItemPrefab, Vector3.zero, Quaternion.identity, scoreList.transform);
            scoreItem.name = $"Score-{player.Name}-{player.Id}";
            scoreItem.text = "0";

            _playerScoreCache.Add(player.Id, (playerItem, scoreItem));
        }

        private void AddPowerUpEntry(PowerUp powerUp)
        {
            var powerUpItem = Instantiate(powerUpItemPrefab, Vector3.zero, Quaternion.identity, powerUpList.transform);
            powerUpItem.name = $"PowerUp-{powerUp.GetPower()}";
            powerUpItem.text = powerUp.ToString();

            _powerUpCache.Add(powerUp, powerUpItem);
        }
        
        private void RemoveScoreEntry((TMP_Text, TMP_Text) entry)
        {
            Destroy(entry.Item1.gameObject);
            Destroy(entry.Item2.gameObject);
        }

        private void RemoveAllScoreEntries()
        {
            foreach (var entry in _playerScoreCache.Values)
                RemoveScoreEntry(entry);

            _playerScoreCache.Clear();
        }

        private void RemoveAllPowerUpEntries()
        {
            foreach (var entry in _powerUpCache.Values)
                Destroy(entry.gameObject);
            
            _powerUpCache.Clear();
        }

        private void DisconnectedFromServerEvent(object sender, DisconnectReason disconnectReason)
        {
            RemoveAllScoreEntries();
            RemoveAllPowerUpEntries();
        }

        private void OnLanguageChanged(object sender, LocaleKey locale)
        {
            var translations = GetComponentsInChildren<TranslatableTMP>();
            foreach (var translation in translations)
                translation.UpdateTranslation();
        }

        [PacketListener(PacketTypeId.PlayerConnected, PacketDirection.Client)]
        public void OnPlayerConnected(PacketPlayerConnected packet)
        {
            AddScoreEntry(packet.Player);
        }

        [PacketListener(PacketTypeId.PlayerDisconnected, PacketDirection.Client)]
        public void OnPlayerDisconnected(PacketPlayerDisconnected packet)
        {
            if (!_playerScoreCache.TryGetValue(packet.Id, out var item))
                return;

            RemoveScoreEntry(item);

            _playerScoreCache.Remove(packet.Id);
        }

        [PacketListener(PacketTypeId.PlayerList, PacketDirection.Client)]
        public void OnPlayerList(PacketPlayerList packet)
        {
            RemoveAllScoreEntries();

            foreach (var player in packet.Players)
                AddScoreEntry(player);

            foreach (var bot in packet.Bots)
                AddScoreEntry(bot);
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