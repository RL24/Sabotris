using System;
using Sabotris.Network;
using UnityEngine;

namespace UI.Menu.Menus
{
    public class MenuPause : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(-9, 2, 4);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(5, 120, 2);

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
                networkController.Client.Shutdown(Reasons.ShutdownClient);
                networkController.Server?.Shutdown(Reasons.ShutdownServer);
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
            return _cameraPosition;
        }
        
        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }
    }
}