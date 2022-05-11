using System;
using Sabotris.Network;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuHostGame : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(5, 11, -3.5f);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(55, -51, -7);

        public MenuSlider sliderBotCount, sliderBotDifficulty, sliderPlayFieldSize, sliderMaxPlayers, sliderBlocksPerShape;
        public MenuToggle toggleGenerateVerticalBlocks, togglePracticeMode;
        public MenuButton buttonCreateLobby, buttonBack;

        public Menu menuMain, menuLobby;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonCreateLobby))
                StartServer();
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        private void StartServer()
        {
            SetButtonsDisabled();

            void ServerStarted(object sender, EventArgs args)
            {
                networkController.Server.OnServerStartEvent -= ServerStarted;

                if (networkController.Server.LobbyId == null)
                {
                    Logging.Error(true, "Started server but no lobby ID");
                    SetButtonsDisabled(false);
                    return;
                }

                networkController.Client?.JoinLobby(networkController.Server.LobbyId.Value);
                menuController.OpenMenu(menuLobby);
            }

            var lobbyData = new LobbyData
            {
                LobbyName = $"{Client.Username}'s Lobby",
                BotCount = (int) sliderBotCount.slider.value,
                BotDifficulty = (int) sliderBotDifficulty.slider.value,
                PlayFieldSize = (int) sliderPlayFieldSize.slider.value,
                MaxPlayers = (int) sliderMaxPlayers.slider.value,
                BlocksPerShape = (int) sliderBlocksPerShape.slider.value,
                GenerateVerticalBlocks = toggleGenerateVerticalBlocks.isToggledOn,
                PracticeMode = togglePracticeMode.isToggledOn
            };

            networkController.Server.OnServerStartEvent += ServerStarted;
            networkController.Server.CreateLobby(lobbyData);
        }

        protected override Menu GetBackMenu()
        {
            return menuMain;
        }

        public override Vector3 GetCameraPosition()
        {
            return _cameraPosition;
        }

        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }
    }
}