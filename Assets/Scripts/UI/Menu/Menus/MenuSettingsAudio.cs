﻿using System;
using Sabotris.IO;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuSettingsAudio : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuSlider sliderMasterVolume, sliderMusicVolume, sliderUIVolume, sliderGameVolume;
        public MenuButton buttonApply,
            buttonBack;

        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            sliderMasterVolume.OnValueChanged += OnMasterVolumeChanged;
            sliderMasterVolume.SetValue(GameSettings.Settings.MasterVolume);
            
            sliderMusicVolume.OnValueChanged += OnMusicVolumeChanged;
            sliderMusicVolume.SetValue(GameSettings.Settings.MusicVolume);
            
            sliderUIVolume.OnValueChanged += OnUIVolumeChanged;
            sliderUIVolume.SetValue(GameSettings.Settings.UIVolume);
            
            sliderGameVolume.OnValueChanged += OnGameVolumeChanged;
            sliderGameVolume.SetValue(GameSettings.Settings.GameVolume);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;

            sliderMasterVolume.OnValueChanged -= OnMasterVolumeChanged;
        }

        private void OnMasterVolumeChanged(object sender, float value)
        {
            GameSettings.Settings.MasterVolume = value;
        }

        private void OnMusicVolumeChanged(object sender, float value)
        {
            GameSettings.Settings.MusicVolume = value;
        }

        private void OnUIVolumeChanged(object sender, float value)
        {
            GameSettings.Settings.UIVolume = value;
        }

        private void OnGameVolumeChanged(object sender, float value)
        {
            GameSettings.Settings.GameVolume = value;
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