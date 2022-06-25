using System;
using System.Linq;
using Sabotris.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Sabotris.Util.Input
{
    public class InputController : MonoBehaviour
    {
        public static float MouseCameraSensitivity => GameSettings.Input.mouseRotateCameraSensitivity; // * 50
        public static float MouseRotateSensitivity => GameSettings.Input.mouseRotateBlockSensitivity; // * 10
        private const float GamepadRotateSensitivity = 90; // / 3.6
        public static float GamepadCameraSensitivity => GameSettings.Input.gamepadRotateCameraSensitivity; // * 50
        
        public InputActionReference
            moveForward,
            moveBack,
            moveLeft,
            moveRight,
            moveDown,
            moveAscend,
            moveDescend,
            moveJump;

        public InputActionReference
            rotateWithMouse,
            rotateYawLeft,
            rotateYawRight,
            rotatePitchUp,
            rotatePitchDown,
            rotateRollLeft,
            rotateRollRight;

        public InputActionReference
            prevContainer,
            nextContainer,
            selectContainer;

        public InputActionReference
            zoomIn,
            zoomOut,
            snapCamera;

        public InputActionReference
            navigateUp,
            navigateDown,
            navigateLeft,
            navigateRight,
            navigateEnter,
            navigateBack;

        public InputActionReference
            gamePause;
        
        private static StickControl GamepadRightStick => Gamepad.current?.rightStick;

        private static Vector2Control MouseDelta => Mouse.current?.delta;
        private static Vector2Control MouseScroll => Mouse.current?.scroll;
        
        private bool IsPressed(InputActionReference actionRef) => actionRef.action.IsPressed();

        private bool WasActionPressed(InputActionReference actionRef) => actionRef.action.WasPressedThisFrame();

        public string GetPath(InputActionReference actionRef, bool gamepad) => actionRef.action.bindings.FirstOrDefault((binding) => binding.effectivePath.StartsWith(gamepad ? "<Gamepad>" : "<Keyboard>")).effectivePath;

        public bool AnyKeyPressed() => Keyboard.current.anyKey.IsPressed();

        public bool AnyGamepadButtonPressed() => Gamepad.current.allControls.Any((x) => x.IsPressed());

        public bool ShouldRotateShape() => IsPressed(rotateWithMouse);

        public bool ShouldPause() => WasActionPressed(gamePause);

        public float GetCameraRotateYaw()
        {
            var mouseValue = MouseDelta?.x.ReadValue() ?? 0;
            var gamepadValue = GamepadRightStick?.x.ReadValue() ?? 0;
            return mouseValue * MouseCameraSensitivity + gamepadValue * GamepadCameraSensitivity;
        }

        public float GetCameraRotatePitch()
        {
            var mouseValue = MouseDelta?.y.ReadValue() ?? 0;
            var gamepadValue = GamepadRightStick?.y.ReadValue() ?? 0;
            return mouseValue * MouseCameraSensitivity + gamepadValue * GamepadCameraSensitivity;
        }
        
        public float GetCameraZoom()
        {
            var mouseValue = (MouseScroll?.y.ReadValue() ?? 0) * (1f / 120f);
            var gamepadValue = (IsPressed(zoomIn).Int() - IsPressed(zoomOut).Int()) * 0.1f;
            return mouseValue + gamepadValue;
        }

        public bool GetSnapCamera() => IsPressed(snapCamera);

        public float GetMoveAdvance()
        {
            var keyboardValue = IsPressed(moveBack).Int() - IsPressed(moveForward).Int();
            return Mathf.Clamp(keyboardValue, -1, 1);
        }
        
        public float GetMoveStrafe()
        {
            var keyboardValue = IsPressed(moveRight).Int() - IsPressed(moveLeft).Int();
            return Mathf.Clamp(keyboardValue, -1, 1);
        }
        
        public float GetMoveAscend()
        {
            var keyboardValue = IsPressed(moveAscend).Int() - IsPressed(moveDescend).Int();
            return Mathf.Clamp(keyboardValue, -1, 1);
        }
        
        public bool GetMoveJump() => WasActionPressed(moveJump);

        public bool GetMoveDown() => IsPressed(moveDown);
        
        public float GetRotateYaw()
        {
            var mouseValue = Mathf.Clamp(MouseDelta?.x.ReadValue() ?? 0, -MouseRotateSensitivity, MouseRotateSensitivity) * ShouldRotateShape().Int();
            var keyboardValue = WasActionPressed(rotateYawRight).Int() - WasActionPressed(rotateYawLeft).Int();
            return mouseValue * MouseRotateSensitivity * ShouldRotateShape().Int() + (keyboardValue) * GamepadRotateSensitivity;
        }
        
        public float GetRotatePitch()
        {
            var mouseValue = Mathf.Clamp(MouseDelta?.y.ReadValue() ?? 0, -MouseRotateSensitivity, MouseRotateSensitivity) * ShouldRotateShape().Int();
            var keyboardValue = WasActionPressed(rotatePitchUp).Int() - WasActionPressed(rotatePitchDown).Int();
            return mouseValue * MouseRotateSensitivity * ShouldRotateShape().Int() + (keyboardValue) * GamepadRotateSensitivity;
        }
        
        public float GetRotateRoll()
        {
            var keyboardValue = WasActionPressed(rotateRollRight).Int() - WasActionPressed(rotateRollLeft).Int();
            return (keyboardValue) * GamepadRotateSensitivity;
        }
        
        public int GetChangeContainerSelection()
        {
            var keyboardValue = IsPressed(nextContainer).Int() - IsPressed(prevContainer).Int();
            return (int) Math.Round(Mathf.Clamp(keyboardValue, -1f, 1f));
        }
        
        public bool GetSelectContainer() => IsPressed(selectContainer);
        
        public float GetMoveUINavigateVertical()
        {
            var keyboardValue = IsPressed(navigateDown).Int() - IsPressed(navigateUp).Int();
            return Mathf.Clamp(keyboardValue, -1, 1);
        }
        
        public float GetMoveUINavigateHorizontal()
        {
            var keyboardValue = IsPressed(navigateRight).Int() - IsPressed(navigateLeft).Int();
            return Mathf.Clamp(keyboardValue, -1, 1);
        }
        
        public bool GetUISelect() => WasActionPressed(navigateEnter);
        
        public bool GetUIBack() => WasActionPressed(navigateBack);
    }
}