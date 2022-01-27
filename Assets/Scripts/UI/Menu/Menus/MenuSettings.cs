using System;
using UnityEngine;

namespace UI.Menu.Menus
{
    public class MenuSettings : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(-9, 2, 4);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(5, 120, 2);

        public MenuButton buttonVideo,
                          buttonAudio,
                          buttonBack;
        
        public Menu menuMain,
                    menuSettingsVideo,
                    menuSettingsAudio,
                    menuPause;

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
            
            if (sender.Equals(buttonVideo))
                menuController.OpenMenu(menuSettingsVideo);
            else if (sender.Equals(buttonAudio))
                menuController.OpenMenu(menuSettingsAudio);
            else if (sender.Equals(buttonBack))
                GoBack();
        }
        
        protected override Menu GetBackMenu()
        {
            return networkController.Client is {IsConnected: true} ? menuPause : menuMain;
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