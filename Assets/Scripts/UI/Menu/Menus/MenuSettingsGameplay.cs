using System;
using Sabotris;
using Sabotris.Util;
using UnityEngine;

namespace UI.Menu.Menus
{
    public class MenuSettingsGameplay : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuButton buttonGameTransitionSpeed,
                          buttonUIAnimationSpeed,
                          buttonGameCameraSpeed,
                          buttonMenuCameraSpeed,
                          buttonBack;
        
        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();
            
            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            if (buttonGameTransitionSpeed is MenuSlider sgts)
            {
                sgts.OnValueChanged += OnGameTransitionSpeedChanged;
                sgts.SetValue(GameSettings.GameTransitionSpeed * 100);
            }

            if (buttonUIAnimationSpeed is MenuSlider suas)
            {
                suas.OnValueChanged += OnUIAnimationSpeed;
                suas.SetValue(GameSettings.UIAnimationSpeed * 100);
            }
            
            if (buttonGameCameraSpeed is MenuSlider sgcs)
            {
                sgcs.OnValueChanged += OnGameCameraSpeedChanged;
                sgcs.SetValue(GameSettings.GameCameraSpeed * 100);
            }

            if (buttonMenuCameraSpeed is MenuSlider smcs)
            {
                smcs.OnValueChanged += OnMenuCameraSpeedChanged;
                smcs.SetValue(GameSettings.MenuCameraSpeed * 100);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
            
            if (buttonGameTransitionSpeed is MenuSlider sgcs)
                sgcs.OnValueChanged -= OnGameTransitionSpeedChanged;
        }

        private void OnGameTransitionSpeedChanged(object sender, float value)
        {
            GameSettings.GameTransitionSpeed = value / 100f;
        }

        private void OnUIAnimationSpeed(object sender, float value)
        {
            GameSettings.UIAnimationSpeed = value / 100f;
        }

        private void OnGameCameraSpeedChanged(object sender, float value)
        {
            GameSettings.GameCameraSpeed = value / 100f;
        }

        private void OnMenuCameraSpeedChanged(object sender, float value)
        {
            GameSettings.MenuCameraSpeed = value / 100f;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonBack))
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