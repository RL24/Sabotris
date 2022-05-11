using System;
using Sabotris.Util;
using Sabotris.Translations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering.Universal;

namespace Sabotris.IO
{
    [Serializable]
    public class GameSettingsConfig
    {
        // Gameplay menu
        public LocaleKey language = LocaleKey.English;
        public float gameTransitionSpeed = 0.4f;
        public float uiAnimationSpeed = 0.2f;
        public float gameCameraSpeed = 0.75f;
        public float menuCameraSpeed = 0.1f;
        
        // Audio menu
        public float masterVolume = 75;
        public float musicVolume = 25;
        public float uiVolume = 100;
        public float gameVolume = 70;
        
        // Video menu
        public bool ambientOcclusion = true;
        public DepthOfFieldMode menuDofMode = DepthOfFieldMode.Bokeh;
        public FullScreenMode fullscreenMode = FullScreenMode.ExclusiveFullScreen;
    }

    [Serializable]
    public class KeyboardBinds
    {
        public Key moveLeft = Key.A;
        public Key moveRight = Key.D;
        public Key moveForward = Key.W;
        public Key moveBack = Key.S;
        public Key moveDown = Key.Space;
        public Key moveAscend = Key.Space;
        public Key moveDescend = Key.LeftShift;

        public Key rotateYawLeft = Key.U;
        public Key rotateYawRight = Key.O;
        public Key rotatePitchUp = Key.I;
        public Key rotatePitchDown = Key.K;
        public Key rotateRollLeft = Key.J;
        public Key rotateRollRight = Key.L;

        public Key navigateLeft = Key.LeftArrow;
        public Key navigateRight = Key.RightArrow;
        public Key navigateUp = Key.UpArrow;
        public Key navigateDown = Key.DownArrow;
        public Key navigateEnter = Key.Enter;
        public Key navigateBack = Key.Escape;

    }

    [Serializable]
    public class GamepadBinds
    {
        public GamepadButton moveAscend = GamepadButton.RightShoulder;
        public GamepadButton moveDescend = GamepadButton.LeftShoulder;
        
        public GamepadButton zoomIn = GamepadButton.LeftStick;
        public GamepadButton zoomOut = GamepadButton.RightStick;
        
        public GamepadButton rotateYawLeft = GamepadButton.LeftShoulder;
        public GamepadButton rotateYawRight = GamepadButton.RightShoulder;
        public GamepadButton rotatePitchUp = GamepadButton.North;
        public GamepadButton rotatePitchDown = GamepadButton.South;
        public GamepadButton rotateRollLeft = GamepadButton.West;
        public GamepadButton rotateRollRight = GamepadButton.East;

        public GamepadButton navigateLeft = GamepadButton.DpadLeft;
        public GamepadButton navigateRight = GamepadButton.DpadRight;
        public GamepadButton navigateUp = GamepadButton.DpadUp;
        public GamepadButton navigateDown = GamepadButton.DpadDown;
        public GamepadButton navigateEnter = GamepadButton.South;
        public GamepadButton navigateBack = GamepadButton.East;
    }

    [Serializable]
    public class GameInputConfig
    {
        public float mouseRotateCameraSensitivity = 1;
        public float mouseRotateBlockSensitivity = 5;

        public float gamepadRotateCameraSensitivity = 1;

        public KeyboardBinds keyboardBinds = new KeyboardBinds();
        public GamepadBinds gamepadBinds = new GamepadBinds();
    }

    public static class GameSettings
    {
        public static event EventHandler OnBeforeSaveEvent;
        public static event EventHandler OnAfterLoadEvent;
        
        public static GameSettingsConfig Settings = new GameSettingsConfig();
        public static GameInputConfig Input = new GameInputConfig();

        public static void Save()
        {
            OnBeforeSaveEvent?.Invoke(null, null);
            FileUtil.SaveGameSettings(Settings);
            FileUtil.SaveGameInput(Input);
        }

        public static void Load()
        {
            Settings = FileUtil.LoadGameSettings() ?? Settings;
            Input = FileUtil.LoadGameInput() ?? Input;
            OnAfterLoadEvent?.Invoke(null, null);
        }
    }
}