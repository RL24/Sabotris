using System;
using Sabotris.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace Sabotris.Util
{
    public static class InputUtil
    {
        #region Keyboard

        private static KeyControl KeyPause => Keyboard.current?.escapeKey;
        
        private static KeyControl KeyLeft => Tern(GameSettings.Input.KeyboardBinds.MoveLeft);
        private static KeyControl KeyRight => Tern(GameSettings.Input.KeyboardBinds.MoveRight);
        private static KeyControl KeyForward => Tern(GameSettings.Input.KeyboardBinds.MoveForward);
        private static KeyControl KeyBackward => Tern(GameSettings.Input.KeyboardBinds.MoveBack);
        
        private static KeyControl KeyMoveDown => Tern(GameSettings.Input.KeyboardBinds.MoveDown);

        private static KeyControl KeyRotateYawLeft => Tern(GameSettings.Input.KeyboardBinds.RotateYawLeft);
        private static KeyControl KeyRotateYawRight => Tern(GameSettings.Input.KeyboardBinds.RotateYawRight);
        private static KeyControl KeyRotatePitchUp => Tern(GameSettings.Input.KeyboardBinds.RotatePitchUp);
        private static KeyControl KeyRotatePitchDown => Tern(GameSettings.Input.KeyboardBinds.RotatePitchDown);
        private static KeyControl KeyRotateRollLeft => Tern(GameSettings.Input.KeyboardBinds.RotateRollLeft);
        private static KeyControl KeyRotateRollRight => Tern(GameSettings.Input.KeyboardBinds.RotateRollRight);

        #region UI

        private static KeyControl KeyNavigateLeft => Tern(GameSettings.Input.KeyboardBinds.NavigateLeft);
        private static KeyControl KeyNavigateRight => Tern(GameSettings.Input.KeyboardBinds.NavigateRight);
        private static KeyControl KeyNavigateUp => Tern(GameSettings.Input.KeyboardBinds.NavigateUp);
        private static KeyControl KeyNavigateDown => Tern(GameSettings.Input.KeyboardBinds.NavigateDown);
        private static KeyControl KeyNavigateEnter => Tern(GameSettings.Input.KeyboardBinds.NavigateEnter);
        private static KeyControl KeyNavigateBack => Tern(GameSettings.Input.KeyboardBinds.NavigateBack);

        #endregion

        #endregion

        #region Mouse

        public static float MouseCameraSensitivity => GameSettings.Input.MouseRotateCameraSensitivity; // * 50
        public static float MouseRotateSensitivity => GameSettings.Input.MouseRotateBlockSensitivity; // * 10

        private static Vector2Control MouseDelta => Mouse.current?.delta;
        private static Vector2Control MouseScroll => Mouse.current?.scroll;
        private static ButtonControl MouseLeftButton => Mouse.current?.leftButton;
        private static ButtonControl MouseRightButton => Mouse.current?.rightButton;

        #endregion

        #region Gamepad

        public const float GamepadRotateSensitivity = 90; // / 3.6
        public static float GamepadCameraSensitivity => GameSettings.Input.GamepadRotateCameraSensitivity; // * 50

        private static ButtonControl GamepadPause => Gamepad.current?.startButton;

        private static StickControl GamepadLeftStick => Gamepad.current?.leftStick;
        private static StickControl GamepadRightStick => Gamepad.current?.rightStick;

        private static ButtonControl GamepadLeftTrigger => Gamepad.current?.leftTrigger;
        private static ButtonControl GamepadRightTrigger => Gamepad.current?.rightTrigger;

        private static ButtonControl GamepadRotateYawLeft => Tern(GameSettings.Input.GamepadBinds.RotateYawLeft);
        private static ButtonControl GamepadRotateYawRight => Tern(GameSettings.Input.GamepadBinds.RotateYawRight);
        private static ButtonControl GamepadRotatePitchUp => Tern(GameSettings.Input.GamepadBinds.RotatePitchUp);
        private static ButtonControl GamepadRotatePitchDown => Tern(GameSettings.Input.GamepadBinds.RotatePitchDown);
        private static ButtonControl GamepadRotateRollLeft => Tern(GameSettings.Input.GamepadBinds.RotateRollLeft);
        private static ButtonControl GamepadRotateRollRight => Tern(GameSettings.Input.GamepadBinds.RotateRollRight);
        
        private static ButtonControl GamepadButtonZoomIn => Tern(GameSettings.Input.GamepadBinds.ZoomIn);
        private static ButtonControl GamepadButtonZoomOut => Tern(GameSettings.Input.GamepadBinds.ZoomOut);
        
        private static ButtonControl GamepadNavigateLeft => Tern(GameSettings.Input.GamepadBinds.NavigateLeft);
        private static ButtonControl GamepadNavigateRight => Tern(GameSettings.Input.GamepadBinds.NavigateRight);
        private static ButtonControl GamepadNavigateUp => Tern(GameSettings.Input.GamepadBinds.NavigateUp);
        private static ButtonControl GamepadNavigateDown => Tern(GameSettings.Input.GamepadBinds.NavigateDown);
        private static ButtonControl GamepadNavigateEnter => Tern(GameSettings.Input.GamepadBinds.NavigateEnter);
        private static ButtonControl GamepadNavigateBack => Tern(GameSettings.Input.GamepadBinds.NavigateBack);

        #endregion

        private static KeyControl Tern(Key key)
        {
            if (key == Key.None)
                return null;
            
            try
            {
                return Keyboard.current == null ? null : Keyboard.current[key];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private static ButtonControl Tern(GamepadButton button)
        {
            try
            {
                return Gamepad.current == null ? null : Gamepad.current[button];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private static bool WasPressed(ButtonControl key) => key?.wasPressedThisFrame ?? false;

        private static bool IsPressed(ButtonControl key) => key?.IsPressed() ?? false;

        public static bool ShouldPause() => WasPressed(KeyPause) || WasPressed(GamepadPause);
        public static bool ShouldRotateShape() => IsPressed(MouseRightButton);

        public static float GetMoveStrafe()
        {
            var keyboardValue = IsPressed(KeyRight).Int() - IsPressed(KeyLeft).Int();
            var gamepadValue = GamepadLeftStick?.x.ReadValue() + (IsPressed(GamepadNavigateRight).Int() - IsPressed(GamepadNavigateLeft).Int()) ?? 0;
            return Mathf.Clamp(keyboardValue + gamepadValue, -1, 1);
        }

        public static float GetMoveAdvance()
        {
            var keyboardValue = IsPressed(KeyBackward).Int() - IsPressed(KeyForward).Int();
            var gamepadValue = GamepadLeftStick?.y.ReadValue() + (IsPressed(GamepadNavigateUp).Int() - IsPressed(GamepadNavigateDown).Int()) ?? 0;
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static bool GetMoveDown()
        {
            return IsPressed(MouseLeftButton) || IsPressed(KeyMoveDown) || IsPressed(GamepadRightTrigger) || IsPressed(GamepadLeftTrigger);
        }

        public static float GetMoveUINavigateVertical()
        {
            var keyboardValue = IsPressed(KeyNavigateDown).Int() - IsPressed(KeyNavigateUp).Int();
            var gamepadValue = GamepadLeftStick?.y.ReadValue() + (IsPressed(GamepadNavigateUp).Int() - IsPressed(GamepadNavigateDown).Int()) ?? 0;
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static float GetMoveUINavigateHorizontal()
        {
            var keyboardValue = IsPressed(KeyNavigateRight).Int() - IsPressed(KeyNavigateLeft).Int();
            var gamepadValue = -GamepadLeftStick?.x.ReadValue() + (IsPressed(GamepadNavigateLeft).Int() - IsPressed(GamepadNavigateRight).Int()) ?? 0;
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static bool GetUISelect()
        {
            return WasPressed(KeyNavigateEnter) || WasPressed(GamepadNavigateEnter);
        }

        public static bool GetUIBack()
        {
            return WasPressed(KeyNavigateBack) || WasPressed(GamepadNavigateBack);
        }

        public static float GetRotateYaw()
        {
            var mouseValue = Mathf.Clamp(MouseDelta?.x.ReadValue() ?? 0, -MouseRotateSensitivity, MouseRotateSensitivity) * ShouldRotateShape().Int();
            var keyboardValue = WasPressed(KeyRotateYawRight).Int() - WasPressed(KeyRotateYawLeft).Int();
            var gamepadValue = WasPressed(GamepadRotateYawRight).Int() - WasPressed(GamepadRotateYawLeft).Int();
            return mouseValue * MouseRotateSensitivity * ShouldRotateShape().Int() + (gamepadValue + keyboardValue) * GamepadRotateSensitivity;
        }

        public static float GetRotatePitch()
        {
            var mouseValue = Mathf.Clamp(MouseDelta?.y.ReadValue() ?? 0, -MouseRotateSensitivity, MouseRotateSensitivity) * ShouldRotateShape().Int();
            var keyboardValue = WasPressed(KeyRotatePitchUp).Int() - WasPressed(KeyRotatePitchDown).Int();
            var gamepadValue = WasPressed(GamepadRotatePitchUp).Int() - WasPressed(GamepadRotatePitchDown).Int();
            return mouseValue * MouseRotateSensitivity * ShouldRotateShape().Int() + (gamepadValue + keyboardValue) * GamepadRotateSensitivity;
        }

        public static float GetRotateRoll()
        {
            var keyboardValue = WasPressed(KeyRotateRollRight).Int() - WasPressed(KeyRotateRollLeft).Int();
            var gamepadValue = WasPressed(GamepadRotateRollRight).Int() - WasPressed(GamepadRotateRollLeft).Int();
            return (gamepadValue + keyboardValue) * GamepadRotateSensitivity;
        }

        public static float GetCameraRotateYaw()
        {
            var mouseValue = MouseDelta?.x.ReadValue() ?? 0;
            var gamepadValue = GamepadRightStick?.x.ReadValue() ?? 0;
            return mouseValue * MouseCameraSensitivity + gamepadValue * GamepadCameraSensitivity;
        }

        public static float GetCameraRotatePitch()
        {
            var mouseValue = MouseDelta?.y.ReadValue() ?? 0;
            var gamepadValue = GamepadRightStick?.y.ReadValue() ?? 0;
            return mouseValue * MouseCameraSensitivity + gamepadValue * GamepadCameraSensitivity;
        }

        public static float GetCameraZoom()
        {
            var mouseValue = (MouseScroll?.y.ReadValue() ?? 0) * (1f / 120f);
            var gamepadValue = (IsPressed(GamepadButtonZoomIn).Int() - IsPressed(GamepadButtonZoomOut).Int()) * 0.1f;
            return mouseValue + gamepadValue;
        }
    }
}