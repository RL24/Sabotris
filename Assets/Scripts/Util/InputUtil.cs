using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Sabotris.Util
{
    public class InputUtil
    {
        #region Keyboard

        public static IEnumerable<KeyControl> KeyEscape => new[] {Keyboard.current?.escapeKey};

        public static IEnumerable<KeyControl> KeyLeft => new[] {Keyboard.current?.aKey, Keyboard.current?.leftArrowKey};

        public static IEnumerable<KeyControl> KeyRight =>
            new[] {Keyboard.current?.dKey, Keyboard.current?.rightArrowKey};

        public static IEnumerable<KeyControl> KeyForward =>
            new[] {Keyboard.current?.wKey, Keyboard.current?.upArrowKey};

        public static IEnumerable<KeyControl> KeyBackward =>
            new[] {Keyboard.current?.sKey, Keyboard.current?.downArrowKey};

        public static IEnumerable<KeyControl> KeySpace => new[] {Keyboard.current?.spaceKey};

        #endregion

        #region Mouse

        public const float MouseCameraSensitivity = 1;
        public const float MouseRotateSensitivity = 10;

        public static Vector2Control MouseDelta => Mouse.current?.delta;
        public static Vector2Control MouseScroll => Mouse.current?.scroll;
        public static ButtonControl MouseLeftButton => Mouse.current?.leftButton;
        public static ButtonControl MouseRightButton => Mouse.current?.rightButton;

        #endregion

        #region Gamepad

        public const float GamepadCameraSensitivity = 1;
        public const float GamepadRotateSensitivity = 90;

        public static ButtonControl GamepadPause => Gamepad.current?.startButton;

        public static StickControl GamepadLeftStick => Gamepad.current?.leftStick;
        public static StickControl GamepadRightStick => Gamepad.current?.rightStick;

        public static ButtonControl GamepadLeftShoulder => Gamepad.current?.leftShoulder;
        public static ButtonControl GamepadRightShoulder => Gamepad.current?.rightShoulder;

        public static IEnumerable<ButtonControl> GamepadLeftTrigger => new[] {Gamepad.current?.leftTrigger};
        public static IEnumerable<ButtonControl> GamepadRightTrigger => new[] {Gamepad.current?.rightTrigger};

        public static ButtonControl GamepadButtonUp => Gamepad.current?.dpad?.up;
        public static ButtonControl GamepadButtonDown => Gamepad.current?.dpad?.down;

        public static ButtonControl GamepadButtonA => Gamepad.current?.buttonSouth;
        public static ButtonControl GamepadButtonB => Gamepad.current?.buttonEast;
        public static ButtonControl GamepadButtonX => Gamepad.current?.buttonWest;
        public static ButtonControl GamepadButtonY => Gamepad.current?.buttonNorth;

        #endregion

        public static bool WasPressed(ButtonControl key) => key?.wasPressedThisFrame ?? false;
        public static bool WasPressed(IEnumerable<ButtonControl> keys) => keys.Any(WasPressed);

        public static bool IsPressed(ButtonControl key) => key?.IsPressed() ?? false;
        public static bool IsPressed(IEnumerable<ButtonControl> keys) => keys.Any(IsPressed);

        public static bool ShouldPause() => WasPressed(KeyEscape) || WasPressed(GamepadPause);
        public static bool ShouldRotateShape() => IsPressed(MouseRightButton);

        private static int Int(bool condition)
        {
            return condition ? 1 : 0;
        }

        public static float GetMoveStrafe()
        {
            var keyboardValue = Int(IsPressed(KeyRight)) - Int(IsPressed(KeyLeft));
            var gamepadValue = GamepadLeftStick?.x.ReadValue() ?? 0;
            return Mathf.Clamp(keyboardValue + gamepadValue, -1, 1);
        }

        public static float GetMoveAdvance()
        {
            var keyboardValue = Int(IsPressed(KeyBackward)) - Int(IsPressed(KeyForward));
            var gamepadValue = GamepadLeftStick?.y.ReadValue() ?? 0;
            return Mathf.Clamp(keyboardValue - gamepadValue, -1, 1);
        }

        public static bool GetMoveDown()
        {
            return IsPressed(MouseLeftButton) || IsPressed(KeySpace) || IsPressed(GamepadRightTrigger) ||
                   IsPressed(GamepadLeftTrigger);
        }

        public static float GetRotateYaw()
        {
            var mouseValue = Mathf.Clamp(MouseDelta?.x.ReadValue() ?? 0, -MouseRotateSensitivity,
                MouseRotateSensitivity);
            var gamepadValue = Int(WasPressed(GamepadRightShoulder)) - Int(WasPressed(GamepadLeftShoulder));
            return mouseValue * MouseRotateSensitivity * Int(ShouldRotateShape()) +
                   gamepadValue * GamepadRotateSensitivity;
        }

        public static float GetRotatePitch()
        {
            var mouseValue = Mathf.Clamp(MouseDelta?.y.ReadValue() ?? 0, -MouseRotateSensitivity,
                MouseRotateSensitivity);
            var gamepadValue = Int(WasPressed(GamepadButtonY)) - Int(WasPressed(GamepadButtonA));
            return mouseValue * MouseRotateSensitivity * Int(ShouldRotateShape()) +
                   gamepadValue * GamepadRotateSensitivity;
        }

        public static float GetRotateRoll()
        {
            return (Int(WasPressed(GamepadButtonB)) - Int(WasPressed(GamepadButtonX))) * GamepadRotateSensitivity;
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
            var gamepadValue = (Int(IsPressed(GamepadButtonDown)) - Int(IsPressed(GamepadButtonUp))) * 0.1f;
            return mouseValue + gamepadValue;
        }
    }
}