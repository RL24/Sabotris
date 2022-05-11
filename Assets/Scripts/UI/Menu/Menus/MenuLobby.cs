using System;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using Steamworks;
using TMPro;
using Translations;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuLobby : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(-3, 8, -17);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(34, 34, 5);

        public MenuChatHistoryItem chatHistoryItemTemplate;

        public SmoothScrollRect chatHistoryScrollBox;
        public RectTransform chatHistory;
        public MenuInput inputChatMessage;
        public MenuButton buttonStartGame, buttonBack;
        
        public TMP_Text botCountText, botDifficultyText, playFieldSizeText, maxPlayersText, blocksPerShapeText, 
            generateVerticalBlocksText, practiceModeText;

        public Menu menuHost, menuJoin;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            inputChatMessage.OnSubmitEvent += OnSubmitChatMessage;

            if (!networkController.Server.Running)
            {
                SelectedButton = -1;
                buttons.Remove(buttonStartGame);
                Destroy(buttonStartGame.gameObject);
            }

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
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
            
            inputChatMessage.OnSubmitEvent -= OnSubmitChatMessage;

            networkController.Client?.DeregisterListener(this);
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonStartGame))
                networkController.Client?.SendPacket(new PacketGameStart());
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        private void OnSubmitChatMessage(object sender, string message)
        {
            if (message.Length == 0)
                return;

            networkController.Client?.SendPacket(new PacketChatMessage
            {
                Id = Guid.NewGuid(),
                Author = Client.UserId,
                Message = message
            });
            if (sender is MenuInput input)
                input.inputField.text = "";
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

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnGameStart(PacketGameStart packet)
        {
            menuController.OpenMenu(null);
        }

        [PacketListener(PacketTypeId.ChatMessage, PacketDirection.Client)]
        public void OnChatMessage(PacketChatMessage packet)
        {
            var chatMessage = Instantiate(chatHistoryItemTemplate, Vector3.zero, Quaternion.identity, chatHistory.transform);
            chatMessage.name = $"ChatMessage-{packet.Id}-{packet.Author}";
            chatMessage.menu = this;
            chatMessage.Author = packet.AuthorName;
            chatMessage.Message = packet.Message;
            
            chatHistory.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Math.Max(500, chatHistory.childCount * 40));
            chatHistoryScrollBox.content.anchoredPosition = new Vector2(0, Math.Max(500, chatHistory.childCount * 40) - 500);
        }
    }
}