using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Sabotris.Util
{
    public static class InputUtil
    {
        #region Keyboard

        private static IEnumerable<KeyControl> KeyEscape => new[] {Keyboard.current?.escapeKey};
        
        private static IEnumerable<KeyControl> KeyLeft => new[] {Keyboard.current?.aKey, Keyboard.current?.leftArrowKey};
        private static IEnumerable<KeyControl> KeyRight => new[] {Keyboard.current?.dKey, Keyboard.current?.rightArrowKey};
        private static IEnumerable<KeyControl> KeyForward => new[] {Keyboard.current?.wKey, Keyboard.current?.upArrowKey};
        private static IEnumerable<KeyControl> KeyBackward => new[] {Keyboard.current?.sKey, Keyboard.current?.downArrowKey};
        
        private static IEnumerable<KeyControl> KeySpace => new[] {Keyboard.current?.spaceKey};

        private static IEnumerable<KeyControl> KeyRotateYawLeft => new[] {Keyboard.current?.uKey};
        private static IEnumerable<KeyControl> KeyRotateYawRight => new[] {Keyboard.current?.oKey};
        private static IEnumerable<KeyControl> KeyRotatePitchUp => new[] {Keyboard.current?.iKey};
        private static IEnumerable<KeyControl> KeyRotatePitchDown => new[] {Keyboard.current?.kKey};
        private static IEnumerable<KeyControl> KeyRotateRollLeft => new[] {Keyboard.current?.jKey};
        private static IEnumerable<KeyControl> KeyRotateRollRight => new[] {Keyboard.current?.lKey};

        #region UI

        private static IEnumerable<KeyControl> KeyUILeft => new[] {Keyboard.current?.leftArrowKey};
        private static IEnumerable<KeyControl> KeyUIRight => new[] {Keyboard.current?.rightArrowKey};
        private static IEnumerable<KeyControl> KeyUIUp => new[] {Keyboard.current?.upArrowKey};
        private static IEnumerable<KeyControl> KeyUIDown => new[] {Keyboard.current?.downArrowKey};

        private static IEnumerable<KeyControl> KeyUISelect => new[] {Keyboard.current?.spaceKey, Keyboard.current?.enterKey};

        #endregion

        #endregion

        #region Mouse

        public static float MouseCameraSensitivity = 1; // * 50
        public static float MouseRotateSensitivity = 5; // * 10

        private static Vector2Control MouseDelta => Mouse.current?.delta;
        private static Vector2Control MouseScroll => Mouse.current?.scroll;
        private static ButtonControl MouseLeftButton => Mouse.current?.leftButton;
        private static ButtonControl MouseRightButton => Mouse.current?.rightButton;

        #endregion

        #region Gamepad

        public static float GamepadCameraSensitivity = 1; // * 50
        public static float GamepadRotateSensitivity = 90; // / 3.6

        private static ButtonControl GamepadPause => Gamepad.current?.startButton;

        private static StickControl GamepadLeftStick => Gamepad.current?.leftStick;
        private static StickControl GamepadRightStick => Gamepad.current?.rightStick;

        private static ButtonControl GamepadLeftShoulder => Gamepad.current?.leftShoulder;
        private static ButtonControl GamepadRightShoulder => Gamepad.current?.rightShoulder;

        private static IEnumerable<ButtonControl> GamepadLeftTrigger => new[] {Gamepad.current?.leftTrigger};
        private static IEnumerable<ButtonControl> GamepadRightTrigger => new[] {Gamepad.current?.rightTrigger};

        private static ButtonControl GamepadButtonUp => Gamepad.current?.dpad?.up;
        private static ButtonControl GamepadButtonDown => Gamepad.current?.dpad?.down;
        private static ButtonControl GamepadButtonLeft => Gamepad.current?.dpad?.left;
        private static ButtonControl GamepadButtonRight => Gamepad.current?.dpad?.right;

        private static ButtonControl GamepadButtonZoomIn => Gamepad.current?.leftStickButton;
        private static ButtonControl GamepadButtonZoomOut => Gamepad.current?.rightStickButton;

        private static ButtonControl GamepadButtonA => Gamepad.current?.buttonSouth;
        private static ButtonControl GamepadButtonB => Gamepad.current?.buttonEast;
        private static ButtonControl GamepadButtonX => Gamepad.current?.buttonWest;
        private static ButtonControl GamepadButtonY => Gamepad.current?.buttonNorth;

        #endregion

        private static bool WasPressed(ButtonControl key) => key?.wasPressedThisFrame ?? false;
        private static bool WasPressed(IEnumerable<ButtonControl> keys) => keys.Any(WasPressed);

        private static bool IsPressed(ButtonControl key) => key?.IsPressed() ?? false;
        private static bool IsPressed(IEnumerable<ButtonControl> keys) => keys.Any(IsPressed);

        public static bool ShouldPause() => WasPressed(KeyEscape) || WasPressed(GamepadPause);
        public static bool ShouldRotateShape() => IsPressed(MouseRightButton);

        public static float GetMoveStrafe()
        {
            var keyboardValue = IsPressed(KeyRight).Int() - IsPressed(KeyLeft).Int();
            var gamepadValue = GamepadLeftStick?.x.ReadValue() + (IsPressed(GamepadButtonRight).Int() - IsPressed(GamepadButtonLeft).Int()) ?? 0;
            return Mathf.Clamp(keyboardValue + gamepadValue, -1, 1);
        }

        public static float GetMoveAdvance()
        {
            var keyboardValue = IsPressed(KeyBackward).Int() - IsPressed(KeyForward).Int();
            var gamepadValue = GamepadLeftStick?.y.ReadValue() + (IsPressed(GamepadButtonUp).Int() - IsPressed(GamepadButtonDown).Int()) ?? 0;
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static bool GetMoveDown()
        {
            return IsPressed(MouseLeftButton) || IsPressed(KeySpace) || IsPressed(GamepadRightTrigger) || IsPressed(GamepadLeftTrigger);
        }

        public static float GetMoveUINavigateVertical()
        {
            var keyboardValue = IsPressed(KeyUIDown).Int() - IsPressed(KeyUIUp).Int();
            var gamepadValue = GamepadLeftStick?.y.ReadValue() + (IsPressed(GamepadButtonUp).Int() - IsPressed(GamepadButtonDown).Int()) ?? 0;
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static float GetMoveUINavigateHorizontal()
        {
            var keyboardValue = IsPressed(KeyUIRight).Int() - IsPressed(KeyUILeft).Int();
            var gamepadValue = -GamepadLeftStick?.x.ReadValue() + (IsPressed(GamepadButtonLeft).Int() - IsPressed(GamepadButtonRight).Int()) ?? 0;
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static bool GetUISelect()
        {
            return WasPressed(KeyUISelect) || WasPressed(GamepadButtonA);
        }

        public static bool GetUIBack()
        {
            return WasPressed(KeyEscape) || WasPressed(GamepadButtonB);
        }

        public static float GetRotateYaw()
        {
            var mouseValue = Mathf.Clamp(MouseDelta?.x.ReadValue() ?? 0, -MouseRotateSensitivity, MouseRotateSensitivity) * ShouldRotateShape().Int();
            var keyboardValue = WasPressed(KeyRotateYawRight).Int() - WasPressed(KeyRotateYawLeft).Int();
            var gamepadValue = WasPressed(GamepadRightShoulder).Int() - WasPressed(GamepadLeftShoulder).Int();
            return mouseValue * MouseRotateSensitivity * ShouldRotateShape().Int() + (gamepadValue + keyboardValue) * GamepadRotateSensitivity;
        }

        public static float GetRotatePitch()
        {
            var mouseValue = Mathf.Clamp(MouseDelta?.y.ReadValue() ?? 0, -MouseRotateSensitivity, MouseRotateSensitivity) * ShouldRotateShape().Int();
            var keyboardValue = WasPressed(KeyRotatePitchUp).Int() - WasPressed(KeyRotatePitchDown).Int();
            var gamepadValue = WasPressed(GamepadButtonY).Int() - WasPressed(GamepadButtonA).Int();
            return mouseValue * MouseRotateSensitivity * ShouldRotateShape().Int() + (gamepadValue + keyboardValue) * GamepadRotateSensitivity;
        }

        public static float GetRotateRoll()
        {
            var keyboardValue = WasPressed(KeyRotateRollRight).Int() - WasPressed(KeyRotateRollLeft).Int();
            var gamepadValue = WasPressed(GamepadButtonB).Int() - WasPressed(GamepadButtonX).Int();
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