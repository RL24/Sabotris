using UnityEngine;

namespace Sabotris
{
    public class GameSettings
    {
        public static float GameTransitionSpeed = 0.4f; // * 100
        public static float UIAnimationSpeed = 0.2f; // * 100
        public static float GameCameraSpeed = 0.75f; // * 100
        public static float MenuCameraSpeed = 0.1f; // * 100

        public static float MasterVolume = 75;

        public static void SetFullscreen(bool full)
        {
            Screen.fullScreen = full;
            if (full)
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
    }
}