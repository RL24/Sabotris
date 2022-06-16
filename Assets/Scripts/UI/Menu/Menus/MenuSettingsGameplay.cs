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
        public MenuToggle toggleTutorial;
        public MenuSlider buttonGameTransitionSpeed,
            buttonUIAnimationSpeed,
            buttonGameCameraSpeed,
            buttonMenuCameraSpeed;
        public MenuButton buttonApply,
            buttonBack;

        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            carouselLanguage.index = (int) GameSettings.Settings.language;
            carouselLanguage.OnValueChanged += OnLanguageChanged;

            toggleTutorial.OnValueChanged += OnTutorialChanged;
            toggleTutorial.isToggledOn = GameSettings.Settings.tutorial;

            buttonGameTransitionSpeed.OnValueChanged += OnGameTransitionSpeedChanged;
            buttonGameTransitionSpeed.SetValue(GameSettings.Settings.gameTransitionSpeed * 100);

            buttonUIAnimationSpeed.OnValueChanged += OnUIAnimationSpeed;
            buttonUIAnimationSpeed.SetValue(GameSettings.Settings.uiAnimationSpeed * 100);

            buttonGameCameraSpeed.OnValueChanged += OnGameCameraSpeedChanged;
            buttonGameCameraSpeed.SetValue(GameSettings.Settings.gameCameraSpeed * 100);

            buttonMenuCameraSpeed.OnValueChanged += OnMenuCameraSpeedChanged;
            buttonMenuCameraSpeed.SetValue(GameSettings.Settings.menuCameraSpeed * 100);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;

            buttonGameTransitionSpeed.OnValueChanged -= OnGameTransitionSpeedChanged;
        }

        private void OnLanguageChanged(object sender, int index)
        {
            GameSettings.Settings.language = (LocaleKey) index;
        }

        private void OnTutorialChanged(object sender, bool value)
        {
            GameSettings.Settings.tutorial = value;
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