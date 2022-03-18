using System;
using Sabotris.Util;
using Translations;
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
        public LocaleKey Language { get; set; } = LocaleKey.English;
        public float GameTransitionSpeed { get; set; } = 0.4f;
        public float UIAnimationSpeed { get; set; } = 0.2f;
        public float GameCameraSpeed { get; set; } = 0.75f;
        public float MenuCameraSpeed { get; set; } = 0.1f;
        
        // Audio menu
        public float MasterVolume { get; set; } = 75;
        
        // Video menu
        public bool AmbientOcclusion { get; set; } = true;
        public DepthOfFieldMode MenuDofMode { get; set; } = DepthOfFieldMode.Bokeh;
        public FullScreenMode FullscreenMode { get; set; } = FullScreenMode.ExclusiveFullScreen;
    }

    [Serializable]
    public class KeyboardBinds
    {
        public Key MoveLeft { get; set; } = Key.A;
        public Key MoveRight { get; set; } = Key.D;
        public Key MoveForward { get; set; } = Key.W;
        public Key MoveBack { get; set; } = Key.S;
        public Key MoveDown { get; set; } = Key.Space;
        public Key MoveAscend { get; set; } = Key.Space;
        public Key MoveDescend { get; set; } = Key.LeftShift;

        public Key RotateYawLeft { get; set; } = Key.U;
        public Key RotateYawRight { get; set; } = Key.O;
        public Key RotatePitchUp { get; set; } = Key.I;
        public Key RotatePitchDown { get; set; } = Key.K;
        public Key RotateRollLeft { get; set; } = Key.J;
        public Key RotateRollRight { get; set; } = Key.L;

        public Key NavigateLeft { get; set; } = Key.LeftArrow;
        public Key NavigateRight { get; set; } = Key.RightArrow;
        public Key NavigateUp { get; set; } = Key.UpArrow;
        public Key NavigateDown { get; set; } = Key.DownArrow;
        public Key NavigateEnter { get; set; } = Key.Enter;
        public Key NavigateBack { get; set; } = Key.Escape;

    }

    [Serializable]
    public class GamepadBinds
    {
        public GamepadButton MoveAscend { get; set; } = GamepadButton.RightShoulder;
        public GamepadButton MoveDescend { get; set; } = GamepadButton.LeftShoulder;
        
        public GamepadButton ZoomIn { get; set; } = GamepadButton.LeftStick;
        public GamepadButton ZoomOut { get; set; } = GamepadButton.RightStick;
        
        public GamepadButton RotateYawLeft { get; set; } = GamepadButton.LeftShoulder;
        public GamepadButton RotateYawRight { get; set; } = GamepadButton.RightShoulder;
        public GamepadButton RotatePitchUp { get; set; } = GamepadButton.North;
        public GamepadButton RotatePitchDown { get; set; } = GamepadButton.South;
        public GamepadButton RotateRollLeft { get; set; } = GamepadButton.West;
        public GamepadButton RotateRollRight { get; set; } = GamepadButton.East;

        public GamepadButton NavigateLeft { get; set; } = GamepadButton.DpadLeft;
        public GamepadButton NavigateRight { get; set; } = GamepadButton.DpadRight;
        public GamepadButton NavigateUp { get; set; } = GamepadButton.DpadUp;
        public GamepadButton NavigateDown { get; set; } = GamepadButton.DpadDown;
        public GamepadButton NavigateEnter { get; set; } = GamepadButton.South;
        public GamepadButton NavigateBack { get; set; } = GamepadButton.East;
    }

    [Serializable]
    public class GameInputConfig
    {
        public float MouseRotateCameraSensitivity { get; set; } = 1;
        public float MouseRotateBlockSensitivity { get; set; } = 5;

        public float GamepadRotateCameraSensitivity { get; set; } = 1;

        public KeyboardBinds KeyboardBinds { get; set; } = new KeyboardBinds();
        public GamepadBinds GamepadBinds { get; set; } = new GamepadBinds();
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