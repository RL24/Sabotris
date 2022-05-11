using System;
using Sabotris.IO;
using Sabotris.Translations;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuSettingsGameplay : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuCarousel carouselLanguage;

        public MenuButton buttonGameTransitionSpeed,
            buttonUIAnimationSpeed,
            buttonGameCameraSpeed,
            buttonMenuCameraSpeed,
            buttonApply,
            buttonBack;

        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            carouselLanguage.index = (int) GameSettings.Settings.language;
            carouselLanguage.OnValueChanged += OnLanguageChanged;

            if (buttonGameTransitionSpeed is MenuSlider sgts)
            {
                sgts.OnValueChanged += OnGameTransitionSpeedChanged;
                sgts.SetValue(GameSettings.Settings.gameTransitionSpeed * 100);
            }

            if (buttonUIAnimationSpeed is MenuSlider suas)
            {
                suas.OnValueChanged += OnUIAnimationSpeed;
                suas.SetValue(GameSettings.Settings.uiAnimationSpeed * 100);
            }

            if (buttonGameCameraSpeed is MenuSlider sgcs)
            {
                sgcs.OnValueChanged += OnGameCameraSpeedChanged;
                sgcs.SetValue(GameSettings.Settings.gameCameraSpeed * 100);
            }

            if (buttonMenuCameraSpeed is MenuSlider smcs)
            {
                smcs.OnValueChanged += OnMenuCameraSpeedChanged;
                smcs.SetValue(GameSettings.Settings.menuCameraSpeed * 100);
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

        private void OnLanguageChanged(object sender, int index)
        {
            GameSettings.Settings.language = (LocaleKey) index;
        }

        private void OnGameTransitionSpeedChanged(object sender, float value)
        {
            GameSettings.Settings.gameTransitionSpeed = value / 100f;
        }

        private void OnUIAnimationSpeed(object sender, float value)
        {
            GameSettings.Settings.uiAnimationSpeed = value / 100f;
        }

        private void OnGameCameraSpeedChanged(object sender, float value)
        {
            GameSettings.Settings.gameCameraSpeed = value / 100f;
        }

        private void OnMenuCameraSpeedChanged(object sender, float value)
        {
            GameSettings.Settings.menuCameraSpeed = value / 100f;
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