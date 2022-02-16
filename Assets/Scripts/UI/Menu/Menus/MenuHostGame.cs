using System;
using Sabotris.Util;
using UnityEngine;

namespace UI.Menu.Menus
{
    public class MenuHostGame : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(5, 11, -3.5f);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(55, -51, -7);

        public MenuButton inputLobbyName, buttonJoinLobby, buttonBack;
        
        public Menu menuMain, menuLobby;

        protected override void Start()
        {
            base.Start();
            
            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            if (inputLobbyName is MenuInput miLobbyName)
                miLobbyName.OnValueChanged += OnLobbyNameValueChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;

            if (inputLobbyName is MenuInput miLobbyName)
                miLobbyName.OnValueChanged -= OnLobbyNameValueChanged;
        }

        private void OnLobbyNameValueChanged(object sender, string args)
        {
            buttonJoinLobby.isDisabled = args.Length == 0;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonJoinLobby))
                StartServer();
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        private void StartServer()
        {
            if (!(inputLobbyName is MenuInput miLobbyName))
                return;
            
            SetButtonsDisabled();

            void ServerStarted(object sender, EventArgs args)
            {
                networkController.Server.OnServerStart -= ServerStarted;

                if (networkController.Server.LobbyId == null)
                {
                    Logging.Error(true, "Started server but no lobby ID");
                    SetButtonsDisabled(false);
                    return;
                }

                networkController.Client.IsHosting = true;
                menuController.OpenMenu(menuLobby);
            }

            networkController.Server.OnServerStart += ServerStarted;
            networkController.Server.CreateLobby(miLobbyName.inputField.text);
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