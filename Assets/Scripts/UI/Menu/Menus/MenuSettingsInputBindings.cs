using System;
using Sabotris.IO;
using Sabotris.Util;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace Sabotris.UI.Menu.Menus
{
    public class MenuSettingsInputBindings : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public MenuBind kMoveLeft, kMoveRight, kMoveForward, kMoveBack,
            kRotateYawLeft, kRotateYawRight, kRotatePitchUp, kRotatePitchDown, kRotateRollLeft, kRotateRollRight,
            kNavigateLeft, kNavigateRight, kNavigateUp, kNavigateDown, kNavigateEnter, kNavigateBack;
        public MenuBind gZoomIn, gZoomOut,
            gRotateYawLeft, gRotateYawRight, gRotatePitchUp, gRotatePitchDown, gRotateRollLeft, gRotateRollRight,
            gNavigateLeft, gNavigateRight, gNavigateUp, gNavigateDown, gNavigateEnter, gNavigateBack;

        public MenuButton buttonApply, buttonBack;

        public Menu menuSettingsControls;

        protected override void Start()
        {
            base.Start();

            foreach (var menuButton in buttons)
            {
                menuButton.OnClick += OnClickButton;
                
                if (!(menuButton is MenuBind mb))
                    continue;
                
                mb.OnKeyBindChangedEvent += OnKeyBindChanged;
                mb.OnGamepadBindChangedEvent += OnGamepadBindChanged;
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
            {
                menuButton.OnClick -= OnClickButton;
                
                if (!(menuButton is MenuBind mb))
                    continue;
                
                mb.OnKeyBindChangedEvent -= OnKeyBindChanged;
                mb.OnGamepadBindChangedEvent -= OnGamepadBindChanged;
            }
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
            return menuSettingsControls;
        }

        public override Vector3 GetCameraPosition()
        {
            return _cameraPosition;
        }

        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }

        private void OnKeyBindChanged(object sender, KeyControl e)
        {
            if (e == null)
                return;
            
            if (sender.Equals(kMoveLeft)) GameSettings.Input.KeyboardBinds.MoveLeft = e.keyCode;
            else if (sender.Equals(kMoveRight)) GameSettings.Input.KeyboardBinds.MoveRight = e.keyCode;
            else if (sender.Equals(kMoveForward)) GameSettings.Input.KeyboardBinds.MoveForward = e.keyCode;
            else if (sender.Equals(kMoveBack)) GameSettings.Input.KeyboardBinds.MoveBack = e.keyCode;

            else if (sender.Equals(kRotateYawLeft)) GameSettings.Input.KeyboardBinds.RotateYawLeft = e.keyCode;
            else if (sender.Equals(kRotateYawRight)) GameSettings.Input.KeyboardBinds.RotateYawRight = e.keyCode;
            else if (sender.Equals(kRotatePitchUp)) GameSettings.Input.KeyboardBinds.RotatePitchUp = e.keyCode;
            else if (sender.Equals(kRotatePitchDown)) GameSettings.Input.KeyboardBinds.RotatePitchDown = e.keyCode;
            else if (sender.Equals(kRotateRollLeft)) GameSettings.Input.KeyboardBinds.RotateRollLeft = e.keyCode;
            else if (sender.Equals(kRotateRollRight)) GameSettings.Input.KeyboardBinds.RotateRollRight = e.keyCode;

            else if (sender.Equals(kNavigateLeft)) GameSettings.Input.KeyboardBinds.NavigateLeft = e.keyCode;
            else if (sender.Equals(kNavigateRight)) GameSettings.Input.KeyboardBinds.NavigateRight = e.keyCode;
            else if (sender.Equals(kNavigateUp)) GameSettings.Input.KeyboardBinds.NavigateUp = e.keyCode;
            else if (sender.Equals(kNavigateDown)) GameSettings.Input.KeyboardBinds.NavigateDown = e.keyCode;
            else if (sender.Equals(kNavigateEnter)) GameSettings.Input.KeyboardBinds.NavigateEnter = e.keyCode;
            else if (sender.Equals(kNavigateBack)) GameSettings.Input.KeyboardBinds.NavigateBack = e.keyCode;

            ((MenuBind) sender).ValueText = e.displayName;
        }

        private void OnGamepadBindChanged(object sender, ButtonControl e)
        {
            if (e == null)
                return;
            
            
            var button = GamepadUtil.GetGamepadButton(e);
            if (button == null)
                return;
            
            if (sender.Equals(gZoomIn)) GameSettings.Input.GamepadBinds.ZoomIn = button.Value;
            else if (sender.Equals(gZoomOut)) GameSettings.Input.GamepadBinds.ZoomOut = button.Value;
            
            else if (sender.Equals(gRotateYawLeft)) GameSettings.Input.GamepadBinds.RotateYawLeft = button.Value;
            else if (sender.Equals(gRotateYawRight)) GameSettings.Input.GamepadBinds.RotateYawRight = button.Value;
            else if (sender.Equals(gRotatePitchUp)) GameSettings.Input.GamepadBinds.RotatePitchUp = button.Value;
            else if (sender.Equals(gRotatePitchDown)) GameSettings.Input.GamepadBinds.RotatePitchDown = button.Value;
            else if (sender.Equals(gRotateRollLeft)) GameSettings.Input.GamepadBinds.RotateRollLeft = button.Value;
            else if (sender.Equals(gRotateRollRight)) GameSettings.Input.GamepadBinds.RotateRollRight = button.Value;

            else if (sender.Equals(gNavigateLeft)) GameSettings.Input.GamepadBinds.NavigateLeft = button.Value;
            else if (sender.Equals(gNavigateRight)) GameSettings.Input.GamepadBinds.NavigateRight = button.Value;
            else if (sender.Equals(gNavigateUp)) GameSettings.Input.GamepadBinds.NavigateUp = button.Value;
            else if (sender.Equals(gNavigateDown)) GameSettings.Input.GamepadBinds.NavigateDown = button.Value;
            else if (sender.Equals(gNavigateEnter)) GameSettings.Input.GamepadBinds.NavigateEnter = button.Value;
            else if (sender.Equals(gNavigateBack)) GameSettings.Input.GamepadBinds.NavigateBack = button.Value;
            
            ((MenuBind) sender).ValueText = e.displayName;
        }
    }
}