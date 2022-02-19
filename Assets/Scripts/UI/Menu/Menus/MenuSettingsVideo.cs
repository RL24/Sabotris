using System;
using UnityEngine;

namespace UI.Menu.Menus
{
    public class MenuSettingsVideo : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuButton buttonFullscreen,
            buttonBack;

        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            if (!(buttonFullscreen is MenuCarousel carouselFullscreen))
                return;

            switch (Screen.fullScreenMode)
            {
                case FullScreenMode.FullScreenWindow:
                    carouselFullscreen.index = 0;
                    break;
                case FullScreenMode.ExclusiveFullScreen:
                    carouselFullscreen.index = 1;
                    break;
                default:
                    carouselFullscreen.index = 2;
                    break;
            }

            carouselFullscreen.OnValueChanged += OnFullscreenModeChanged;
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

            if (sender.Equals(buttonBack))
                GoBack();
        }

        private void OnFullscreenModeChanged(object sender, int index)
        {
            switch (index)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }

        protected override Menu GetBackMenu()
        {
            return menuSettings;
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