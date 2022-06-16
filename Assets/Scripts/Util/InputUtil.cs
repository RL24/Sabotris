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

        private static KeyControl KeyLeft => Tern(GameSettings.Input.keyboardBinds.moveLeft);
        private static KeyControl KeyRight => Tern(GameSettings.Input.keyboardBinds.moveRight);
        private static KeyControl KeyForward => Tern(GameSettings.Input.keyboardBinds.moveForward);
        private static KeyControl KeyBackward => Tern(GameSettings.Input.keyboardBinds.moveBack);
        private static KeyControl KeyAscend => Tern(GameSettings.Input.keyboardBinds.moveAscend);
        private static KeyControl KeyDescend => Tern(GameSettings.Input.keyboardBinds.moveDescend);
        private static KeyControl KeyJump => Tern(GameSettings.Input.keyboardBinds.moveJump);

        private static KeyControl KeyMoveDown => Tern(GameSettings.Input.keyboardBinds.moveDown);

        private static KeyControl KeyRotateYawLeft => Tern(GameSettings.Input.keyboardBinds.rotateYawLeft);
        private static KeyControl KeyRotateYawRight => Tern(GameSettings.Input.keyboardBinds.rotateYawRight);
        private static KeyControl KeyRotatePitchUp => Tern(GameSettings.Input.keyboardBinds.rotatePitchUp);
        private static KeyControl KeyRotatePitchDown => Tern(GameSettings.Input.keyboardBinds.rotatePitchDown);
        private static KeyControl KeyRotateRollLeft => Tern(GameSettings.Input.keyboardBinds.rotateRollLeft);
        private static KeyControl KeyRotateRollRight => Tern(GameSettings.Input.keyboardBinds.rotateRollRight);
        
        private static KeyControl KeySnapCamera => Tern(GameSettings.Input.keyboardBinds.snapCamera);
        
        #region Sabotage

        private static KeyControl KeyPreviousContainer => Tern(GameSettings.Input.keyboardBinds.previousContainer);
        private static KeyControl KeyNextContainer => Tern(GameSettings.Input.keyboardBinds.nextContainer);
        private static KeyControl KeySelectContainer => Tern(GameSettings.Input.keyboardBinds.selectContainer);

        #endregion

        #region UI

        private static KeyControl KeyNavigateLeft => Tern(GameSettings.Input.keyboardBinds.navigateLeft);
        private static KeyControl KeyNavigateRight => Tern(GameSettings.Input.keyboardBinds.navigateRight);
        private static KeyControl KeyNavigateUp => Tern(GameSettings.Input.keyboardBinds.navigateUp);
        private static KeyControl KeyNavigateDown => Tern(GameSettings.Input.keyboardBinds.navigateDown);
        private static KeyControl KeyNavigateEnter => Tern(GameSettings.Input.keyboardBinds.navigateEnter);
        private static KeyControl KeyNavigateBack => Tern(GameSettings.Input.keyboardBinds.navigateBack);

        #endregion

        #endregion

        #region Mouse

        public static float MouseCameraSensitivity => GameSettings.Input.mouseRotateCameraSensitivity; // * 50
        public static float MouseRotateSensitivity => GameSettings.Input.mouseRotateBlockSensitivity; // * 10

        private static Vector2Control MouseDelta => Mouse.current?.delta;
        private static Vector2Control MouseScroll => Mouse.current?.scroll;
        private static ButtonControl MouseLeftButton => Mouse.current?.leftButton;
        private static ButtonControl MouseMiddleButton => Mouse.current?.middleButton;
        private static ButtonControl MouseRightButton => Mouse.current?.rightButton;

        #endregion

        #region Gamepad

        private const float GamepadRotateSensitivity = 90; // / 3.6
        public static float GamepadCameraSensitivity => GameSettings.Input.gamepadRotateCameraSensitivity; // * 50

        private static ButtonControl GamepadPause => Gamepad.current?.startButton;

        private static StickControl GamepadLeftStick => Gamepad.current?.leftStick;
        private static StickControl GamepadRightStick => Gamepad.current?.rightStick;

        private static ButtonControl GamepadLeftTrigger => Gamepad.current?.leftTrigger;
        private static ButtonControl GamepadRightTrigger => Gamepad.current?.rightTrigger;

        private static ButtonControl GamepadAscend => Tern(GameSettings.Input.gamepadBinds.moveAscend);
        private static ButtonControl GamepadDescend => Tern(GameSettings.Input.gamepadBinds.moveDescend);
        private static ButtonControl GamepadJump => Tern(GameSettings.Input.gamepadBinds.moveJump);

        private static ButtonControl GamepadRotateYawLeft => Tern(GameSettings.Input.gamepadBinds.rotateYawLeft);
        private static ButtonControl GamepadRotateYawRight => Tern(GameSettings.Input.gamepadBinds.rotateYawRight);
        private static ButtonControl GamepadRotatePitchUp => Tern(GameSettings.Input.gamepadBinds.rotatePitchUp);
        private static ButtonControl GamepadRotatePitchDown => Tern(GameSettings.Input.gamepadBinds.rotatePitchDown);
        private static ButtonControl GamepadRotateRollLeft => Tern(GameSettings.Input.gamepadBinds.rotateRollLeft);
        private static ButtonControl GamepadRotateRollRight => Tern(GameSettings.Input.gamepadBinds.rotateRollRight);

        private static ButtonControl GamepadButtonZoomIn => Tern(GameSettings.Input.gamepadBinds.zoomIn);
        private static ButtonControl GamepadButtonZoomOut => Tern(GameSettings.Input.gamepadBinds.zoomOut);
        
        #region Sabotage

        private static ButtonControl GamepadPreviousContainer => Tern(GameSettings.Input.gamepadBinds.previousContainer);
        private static ButtonControl GamepadNextContainer => Tern(GameSettings.Input.gamepadBinds.nextContainer);
        private static ButtonControl GamepadSelectContainer => Tern(GameSettings.Input.gamepadBinds.selectContainer);

        #endregion

        #region UI
        
        private static ButtonControl GamepadNavigateLeft => Tern(GameSettings.Input.gamepadBinds.navigateLeft);
        private static ButtonControl GamepadNavigateRight => Tern(GameSettings.Input.gamepadBinds.navigateRight);
        private static ButtonControl GamepadNavigateUp => Tern(GameSettings.Input.gamepadBinds.navigateUp);
        private static ButtonControl GamepadNavigateDown => Tern(GameSettings.Input.gamepadBinds.navigateDown);
        private static ButtonControl GamepadNavigateEnter => Tern(GameSettings.Input.gamepadBinds.navigateEnter);
        private static ButtonControl GamepadNavigateBack => Tern(GameSettings.Input.gamepadBinds.navigateBack);

        #endregion
        
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
            var gamepadValue = (GamepadLeftStick?.x.ReadValue() ?? 0) + (IsPressed(GamepadNavigateRight).Int() - IsPressed(GamepadNavigateLeft).Int());
            return Mathf.Clamp(keyboardValue + gamepadValue, -1, 1);
        }

        public static float GetMoveAdvance()
        {
            var keyboardValue = IsPressed(KeyBackward).Int() - IsPressed(KeyForward).Int();
            var gamepadValue = (GamepadLeftStick?.y.ReadValue() ?? 0) + (IsPressed(GamepadNavigateUp).Int() - IsPressed(GamepadNavigateDown).Int());
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static float GetMoveAscend()
        {
            var keyboardValue = IsPressed(KeyAscend).Int() - IsPressed(KeyDescend).Int();
            var gamepadValue = IsPressed(GamepadDescend).Int() - IsPressed(GamepadAscend).Int();
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static bool GetMoveJump()
        {
            return WasPressed(KeyJump) || WasPressed(GamepadJump);
        }

        public static bool GetMoveDown()
        {
            return IsPressed(MouseLeftButton) || IsPressed(KeyMoveDown) || IsPressed(GamepadRightTrigger);
        }

        public static bool GetSnapCamera()
        {
            return IsPressed(MouseMiddleButton) || IsPressed(KeySnapCamera) || IsPressed(GamepadLeftTrigger);
        }

        public static int GetChangeContainerSelection()
        {
            var keyboardValue = IsPressed(KeyNextContainer).Int() - IsPressed(KeyPreviousContainer).Int();
            var gamepadValue = (GamepadLeftStick?.x.ReadValue() ?? 0) + IsPressed(GamepadPreviousContainer).Int() - IsPressed(GamepadNextContainer).Int();
            return (int) Math.Round(Mathf.Clamp(keyboardValue - gamepadValue, -1, 1));
        }

        public static bool GetSelectContainer()
        {
            return IsPressed(KeySelectContainer) || IsPressed(GamepadSelectContainer);
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