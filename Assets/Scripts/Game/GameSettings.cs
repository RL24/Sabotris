using UnityEngine;

namespace Sabotris
{
    public class GameSettings
    {
        public const float GameTransitionSpeed = 0.2f;
        public const float UIAnimationSpeed = 0.2f;
        public static readonly float GameCameraSpeed = 0.75f;
        public static readonly float MenuCameraSpeed = 0.1f;

        public static float MasterVolume = 75;

        public static void SetFullscreen(bool full)
        {
            Screen.fullScreen = full;
            if (full)
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
    }
}