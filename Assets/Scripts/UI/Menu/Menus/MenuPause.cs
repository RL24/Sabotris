using System;
using Sabotris.Network;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuPause : Menu
    {
        public MenuButton buttonDisconnect,
            buttonSettings,
            buttonBack;

        public Menu menuMain,
            menuSettings;

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

            if (sender.Equals(buttonDisconnect))
            {
                networkController.Client.DisconnectSocket(DisconnectReason.ClientDisconnected);
                networkController.Server.DisconnectSockets(DisconnectReason.ServerClosed);
                menuController.OpenMenu(menuMain);
            }
            else if (sender.Equals(buttonSettings))
                menuController.OpenMenu(menuSettings);
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        protected override Menu GetBackMenu()
        {
            return null;
        }

        public override Vector3 GetCameraPosition()
        {
            return cameraController.cameraPosition;
        }

        public override Quaternion GetCameraRotation()
        {
            return cameraController.cameraRotation;
        }
    }
}