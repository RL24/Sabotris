using System;
using UnityEngine;

namespace Menu.Menus
{
    public class MenuMain : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(-4.5f, 7.5f, -10);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(30, 30, 4);

        public MenuButton buttonHost,
                          buttonJoin,
                          buttonSettings,
                          buttonExit;
        
        public Menu menuHostGame, menuJoinGame, menuSettings;

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

            if (sender.Equals(buttonHost))
                menuController.OpenMenu(menuHostGame);
            else if (sender.Equals(buttonJoin))
                menuController.OpenMenu(menuJoinGame);
            else if (sender.Equals(buttonSettings))
                menuController.OpenMenu(menuSettings);
            else if (sender.Equals(buttonExit))
                Application.Quit();
        }
        
        protected override void GoBack()
        {
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