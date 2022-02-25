using System;
using System.Linq;
using Sabotris.Network;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuGameOver : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(0, 8, -35);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(0, 0, 0);

        public MenuButton buttonLeave;

        public Menu menuMain;

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

            if (sender.Equals(buttonLeave))
            {
                networkController.Client.DisconnectSocket(DisconnectReason.ClientDisconnected);
                networkController.Server.DisconnectSockets(DisconnectReason.ServerClosed);
                menuController.OpenMenu(menuMain);
            }
        }

        protected override void GoBack()
        {
        }

        public override Vector3 GetCameraPosition()
        {
            var containers = world.Containers.Values;
            var center = containers.Average((container) => container.transform.position.x);
            return new Vector3(center, _cameraPosition.y, _cameraPosition.z);
        }

        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }
    }
}