using System;
using Sabotris;
using UnityEngine;

namespace Menu.Menus
{
    public class MenuSettingsAudio : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuButton buttonMasterVolume, buttonBack;
        
        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();
            
            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            if (buttonMasterVolume is MenuSlider sliderMasterVolume)
            {
                sliderMasterVolume.OnValueChanged += OnMasterVolumeChanged;
                sliderMasterVolume.SetValue(GameSettings.MasterVolume);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
            
            if (buttonMasterVolume is MenuSlider sliderMasterVolume)
                sliderMasterVolume.OnValueChanged -= OnMasterVolumeChanged;
        }

        private void OnMasterVolumeChanged(object sender, float value)
        {
            GameSettings.MasterVolume = value;
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