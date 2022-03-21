using System;
using Sabotris.IO;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuSettingsControls : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuButton buttonInputBindings,
            buttonGamepadCameraSensitivity,
            buttonMouseCameraSensitivity,
            buttonMouseRotateSensitivity,
            buttonApply,
            buttonBack;

        public Menu menuInputBindings, menuSettings;

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
            GameSettings.Input.gamepadRotateCameraSensitivity = value / 50f;
        }

        private void OnMouseCameraSensitivityChanged(object sender, float value)
        {
            GameSettings.Input.mouseRotateCameraSensitivity = value / 50f;
        }

        private void OnMouseRotateSensitivityChanged(object sender, float value)
        {
            GameSettings.Input.mouseRotateBlockSensitivity = value / 10f;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;
            if (sender.Equals(buttonInputBindings))
                menuController.OpenMenu(menuInputBindings);
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