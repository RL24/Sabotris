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
            
            if (sender.Equals(kMoveLeft)) GameSettings.Input.keyboardBinds.moveLeft = e.keyCode;
            else if (sender.Equals(kMoveRight)) GameSettings.Input.keyboardBinds.moveRight = e.keyCode;
            else if (sender.Equals(kMoveForward)) GameSettings.Input.keyboardBinds.moveForward = e.keyCode;
            else if (sender.Equals(kMoveBack)) GameSettings.Input.keyboardBinds.moveBack = e.keyCode;

            else if (sender.Equals(kRotateYawLeft)) GameSettings.Input.keyboardBinds.rotateYawLeft = e.keyCode;
            else if (sender.Equals(kRotateYawRight)) GameSettings.Input.keyboardBinds.rotateYawRight = e.keyCode;
            else if (sender.Equals(kRotatePitchUp)) GameSettings.Input.keyboardBinds.rotatePitchUp = e.keyCode;
            else if (sender.Equals(kRotatePitchDown)) GameSettings.Input.keyboardBinds.rotatePitchDown = e.keyCode;
            else if (sender.Equals(kRotateRollLeft)) GameSettings.Input.keyboardBinds.rotateRollLeft = e.keyCode;
            else if (sender.Equals(kRotateRollRight)) GameSettings.Input.keyboardBinds.rotateRollRight = e.keyCode;

            else if (sender.Equals(kNavigateLeft)) GameSettings.Input.keyboardBinds.navigateLeft = e.keyCode;
            else if (sender.Equals(kNavigateRight)) GameSettings.Input.keyboardBinds.navigateRight = e.keyCode;
            else if (sender.Equals(kNavigateUp)) GameSettings.Input.keyboardBinds.navigateUp = e.keyCode;
            else if (sender.Equals(kNavigateDown)) GameSettings.Input.keyboardBinds.navigateDown = e.keyCode;
            else if (sender.Equals(kNavigateEnter)) GameSettings.Input.keyboardBinds.navigateEnter = e.keyCode;
            else if (sender.Equals(kNavigateBack)) GameSettings.Input.keyboardBinds.navigateBack = e.keyCode;

            ((MenuBind) sender).ValueText = e.displayName;
        }

        private void OnGamepadBindChanged(object sender, ButtonControl e)
        {
            if (e == null)
                return;
            
            
            var button = GamepadUtil.GetGamepadButton(e);
            if (button == null)
                return;
            
            if (sender.Equals(gZoomIn)) GameSettings.Input.gamepadBinds.zoomIn = button.Value;
            else if (sender.Equals(gZoomOut)) GameSettings.Input.gamepadBinds.zoomOut = button.Value;
            
            else if (sender.Equals(gRotateYawLeft)) GameSettings.Input.gamepadBinds.rotateYawLeft = button.Value;
            else if (sender.Equals(gRotateYawRight)) GameSettings.Input.gamepadBinds.rotateYawRight = button.Value;
            else if (sender.Equals(gRotatePitchUp)) GameSettings.Input.gamepadBinds.rotatePitchUp = button.Value;
            else if (sender.Equals(gRotatePitchDown)) GameSettings.Input.gamepadBinds.rotatePitchDown = button.Value;
            else if (sender.Equals(gRotateRollLeft)) GameSettings.Input.gamepadBinds.rotateRollLeft = button.Value;
            else if (sender.Equals(gRotateRollRight)) GameSettings.Input.gamepadBinds.rotateRollRight = button.Value;

            else if (sender.Equals(gNavigateLeft)) GameSettings.Input.gamepadBinds.navigateLeft = button.Value;
            else if (sender.Equals(gNavigateRight)) GameSettings.Input.gamepadBinds.navigateRight = button.Value;
            else if (sender.Equals(gNavigateUp)) GameSettings.Input.gamepadBinds.navigateUp = button.Value;
            else if (sender.Equals(gNavigateDown)) GameSettings.Input.gamepadBinds.navigateDown = button.Value;
            else if (sender.Equals(gNavigateEnter)) GameSettings.Input.gamepadBinds.navigateEnter = button.Value;
            else if (sender.Equals(gNavigateBack)) GameSettings.Input.gamepadBinds.navigateBack = button.Value;
            
            ((MenuBind) sender).ValueText = e.displayName;
        }
    }
}