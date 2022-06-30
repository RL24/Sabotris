using System;
using System.Collections;
using System.Linq;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Chat;
using Sabotris.Network.Packets.Game;
using Sabotris.Network.Packets.Players;
using Sabotris.Translations;
using TMPro;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuLobby : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(-3, 8, -17);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(34, 34, 5);

        public MenuCountdown countdownTimer;
        public MenuToggle toggleReady;
        public MenuButton buttonBack;

        public TMP_Text botCountText,
            botDifficultyText,
            playFieldSizeText,
            maxPlayersText,
            blocksPerShapeText,
            generateVerticalBlocksText,
            practiceModeText,
            powerUpsText,
            powerUpAutoPickDelayText;

        public Menu menuHost, menuJoin;

        private Coroutine _countdownCoroutine;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            toggleReady.OnValueChanged += OnReadyChanged;

            networkController.Client?.RegisterListener(this);

            var data = networkController.Client?.LobbyData;
            if (data == null)
                return;

            if (botCountText)
                botCountText.text = Localization.Translate(TranslationKey.UiMenuDisplayBotCount, data.BotCount);

            if (botDifficultyText)
                botDifficultyText.text = Localization.Translate(TranslationKey.UiMenuDisplayBotDifficulty, data.BotDifficulty);

            if (playFieldSizeText)
                playFieldSizeText.text = Localization.Translate(TranslationKey.UiMenuDisplayPlayFieldSize, data.PlayFieldSize * 2 + 1);

            if (maxPlayersText)
                maxPlayersText.text = Localization.Translate(TranslationKey.UiMenuDisplayMaxPlayers, data.MaxPlayers);

            if (blocksPerShapeText)
                blocksPerShapeText.text = Localization.Translate(TranslationKey.UiMenuDisplayBlocksPerShape, data.BlocksPerShape);

            if (generateVerticalBlocksText)
                generateVerticalBlocksText.text = Localization.Translate(TranslationKey.UiMenuDisplayGenerateVerticalBlocks, Localization.Translate(data.GenerateVerticalBlocks ? TranslationKey.UiYes : TranslationKey.UiNo));

            if (practiceModeText)
                practiceModeText.text = Localization.Translate(TranslationKey.UiMenuDisplayPracticeMode, Localization.Translate(data.PracticeMode ? TranslationKey.UiYes : TranslationKey.UiNo));

            if (powerUpsText)
                powerUpsText.text = Localization.Translate(TranslationKey.UiMenuDisplayPowerUps, data.PowerUps);
            
            if (powerUpAutoPickDelayText)
                powerUpAutoPickDelayText.text = Localization.Translate(TranslationKey.UiMenuDisplayPowerUpAutoPickDelay, data.PowerUpAutoPickDelay);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;

            networkController.Client?.DeregisterListener(this);
        }

        private void OnReadyChanged(object sender, bool ready)
        {
            networkController.Client?.SendPacket(new PacketPlayerReady
            {
                Id = Client.UserId,
                Ready = ready
            });
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonBack))
                GoBack();
        }

        protected override void GoBack()
        {
            base.GoBack();
            networkController.Client?.DisconnectSocket(DisconnectReason.ClientDisconnected);
            networkController.Server.DisconnectSockets(DisconnectReason.ServerClosed);
        }

        protected override Menu GetBackMenu()
        {
            return networkController.Server.Running ? menuHost : menuJoin;
        }

        public override Vector3 GetCameraPosition()
        {
            return _cameraPosition;
        }

        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }

        private IEnumerator StartCountdown()
        {
            networkController.Server?.SetLobbyPrivacy(true);
            
            yield return countdownTimer.StartCountdown();
            
            if (networkController.Server?.Running == true)
                networkController.Client?.SendPacket(new PacketGameStart());
        }

        private void StopCountdown()
        {
            networkController.Server?.SetLobbyPrivacy(false);
            
            countdownTimer.StopCountdown();
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnGameStart(PacketGameStart packet)
        {
            menuController.OpenMenu(null);
        }

        [PacketListener(PacketTypeId.PlayerConnected, PacketDirection.Client)]
        [PacketListener(PacketTypeId.PlayerDisconnected, PacketDirection.Client)]
        public void OnPlayerConnectionChanged(Packet packet)
        {
            StopCountdown();
        }

        [PacketListener(PacketTypeId.PlayerReady, PacketDirection.Client)]
        public void OnPlayerReady(PacketPlayerReady packet)
        {
            var allReady = world.Containers.All((p) => p.ready);
            if (allReady)
            {
                if (_countdownCoroutine != null)
                    StopCountdown();
                _countdownCoroutine = StartCoroutine(StartCountdown());
            } else StopCountdown();
        }
    }
}