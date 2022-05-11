using System;
using Sabotris.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuSettingsVideo : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuButton buttonSsao,
            buttonMenuDof,
            buttonFullscreen,
            buttonApply,
            buttonBack;

        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            if (buttonSsao is MenuToggle toggleSsao)
            {
                toggleSsao.isToggledOn = GameSettings.Settings.ambientOcclusion;
                toggleSsao.OnValueChanged += OnSsaoToggled;
            }

            if (buttonMenuDof is MenuCarousel carouselMenuDof)
            {
                carouselMenuDof.index = GameSettings.Settings.menuDofMode switch
                {
                    DepthOfFieldMode.Off => 0,
                    DepthOfFieldMode.Gaussian => 1,
                    DepthOfFieldMode.Bokeh => 2,
                    _ => carouselMenuDof.index
                };

                carouselMenuDof.OnValueChanged += OnMenuDofModeChanged;
            }

            if (buttonFullscreen is MenuCarousel carouselFullscreen)
            {
                carouselFullscreen.index = GameSettings.Settings.fullscreenMode switch
                {
                    FullScreenMode.FullScreenWindow => 0,
                    FullScreenMode.ExclusiveFullScreen => 1,
                    _ => 2
                };

                carouselFullscreen.OnValueChanged += OnFullscreenModeChanged;
            }
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

            if (sender.Equals(buttonApply))
                Save();
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        private void OnSsaoToggled(object sender, bool active)
        {
            GameSettings.Settings.ambientOcclusion = active;
        }

        private void OnMenuDofModeChanged(object sender, int index)
        {
            GameSettings.Settings.menuDofMode = index switch
            {
                0 => DepthOfFieldMode.Off,
                1 => DepthOfFieldMode.Gaussian,
                2 => DepthOfFieldMode.Bokeh,
                _ => GameSettings.Settings.menuDofMode
            };
        }

        private void OnFullscreenModeChanged(object sender, int index)
        {
            GameSettings.Settings.fullscreenMode = index switch
            {
                0 => FullScreenMode.FullScreenWindow,
                1 => FullScreenMode.ExclusiveFullScreen,
                2 => FullScreenMode.Windowed,
                _ => GameSettings.Settings.fullscreenMode
            };
        }

        private void Save()
        {
            GameSettings.Save();
            GoBack();
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