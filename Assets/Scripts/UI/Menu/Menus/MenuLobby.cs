using System;
using System.Collections;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Chat;
using Sabotris.Network.Packets.Game;
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

            if (networkController.Server != null)
            {
                networkController.Server.OnStartMatchCountdown += OnStartMatchCountdown;
                networkController.Server.OnStopMatchCountdown += OnStopMatchCountdown;
            }

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
            
            if (networkController.Server != null)
            {
                networkController.Server.OnStartMatchCountdown -= OnStartMatchCountdown;
                networkController.Server.OnStopMatchCountdown -= OnStopMatchCountdown;
            }
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

        private void OnStartMatchCountdown(object sender, EventArgs eventArgs)
        {
            if (_countdownCoroutine != null)
                OnStopMatchCountdown(null, null);
            _countdownCoroutine = StartCoroutine(StartCountdown());
        }

        private void OnStopMatchCountdown(object sender, EventArgs eventArgs)
        {
            countdownTimer.StopCountdown();
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }
        }

        private IEnumerator StartCountdown()
        {
            yield return countdownTimer.StartCountdown();
            
            networkController.Client?.SendPacket(new PacketGameStart());
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnGameStart(PacketGameStart packet)
        {
            menuController.OpenMenu(null);
        }
    }
}