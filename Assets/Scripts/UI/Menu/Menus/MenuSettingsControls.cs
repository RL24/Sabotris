using System;
using Sabotris.Util;
using UnityEngine;

namespace UI.Menu.Menus
{
    public class MenuSettingsControls : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuButton buttonGamepadCameraSensitivity,
                          buttonGamepadRotateSensitivity,
                          buttonMouseCameraSensitivity,
                          buttonMouseRotateSensitivity,
                          buttonBack;
        
        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();
            
            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            if (buttonGamepadCameraSensitivity is MenuSlider sgcs)
            {
                sgcs.OnValueChanged += OnGamepadCameraSensitivityChanged;
                sgcs.SetValue(InputUtil.GamepadCameraSensitivity * 50);
            }

            if (buttonGamepadRotateSensitivity is MenuSlider sgrs)
            {
                sgrs.OnValueChanged += OnGamepadRotateSensitivityChanged;
                sgrs.SetValue(InputUtil.GamepadRotateSensitivity / 3.6f);
            }
            
            if (buttonMouseCameraSensitivity is MenuSlider smcs)
            {
                smcs.OnValueChanged += OnMouseCameraSensitivityChanged;
                smcs.SetValue(InputUtil.MouseCameraSensitivity * 50);
            }

            if (buttonMouseRotateSensitivity is MenuSlider smrs)
            {
                smrs.OnValueChanged += OnMouseRotateSensitivityChanged;
                smrs.SetValue(InputUtil.MouseRotateSensitivity * 10);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
            
            if (buttonGamepadCameraSensitivity is MenuSlider sgcs)
                sgcs.OnValueChanged -= OnGamepadCameraSensitivityChanged;
        }

        private void OnGamepadCameraSensitivityChanged(object sender, float value)
        {
            InputUtil.GamepadCameraSensitivity = value / 50f;
        }

        private void OnGamepadRotateSensitivityChanged(object sender, float value)
        {
            InputUtil.GamepadRotateSensitivity = value * 3.6f;
        }

        private void OnMouseCameraSensitivityChanged(object sender, float value)
        {
            InputUtil.MouseCameraSensitivity = value / 50f;
        }

        private void OnMouseRotateSensitivityChanged(object sender, float value)
        {
            InputUtil.MouseRotateSensitivity = value / 10f;
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