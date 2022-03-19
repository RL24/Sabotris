using Sabotris.IO;
using UnityEngine;

namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        public AudioSource hoverButton, clickButton,
            refreshingLobbies,
            playerJoinLobby, playerLeaveLobby,
            shapeDrop, shapeMove, shapeRotate, shapeLock,
            layerDelete,
            playerDie,
            gameOver,
            music;

        private void Update()
        {
            music.volume = GetMusicVolume();
        }

        private static float GetMasterVolume()
        {
            return GameSettings.Settings.MasterVolume * 0.01f;
        }

        private static float GetMusicVolume()
        {
            return GameSettings.Settings.MusicVolume * 0.01f * GetMasterVolume();
        }

        private static float GetUIVolume()
        {
            return GameSettings.Settings.UIVolume * 0.01f * GetMasterVolume();
        }

        public static float GetGameVolume()
        {
            return GameSettings.Settings.GameVolume * 0.01f * GetMasterVolume();
        }

        public static float GetButtonClickVolume()
        {
            return 0.5f * GetUIVolume();
        }

        public static float GetButtonClickPitch()
        {
            return Random.Range(1, 1.2f);
        }

        public static float GetButtonHoverVolume()
        {
            return 0.3f * GetUIVolume();
        }

        public static float GetButtonHoverPitch()
        {
            return Random.Range(1.4f, 1.8f);
        }

        public static float GetShapeMovePitch()
        {
            return Random.Range(1, 1.4f);
        }

        public static float GetShapeRotatePitch()
        {
            return Random.Range(1.4f, 1.8f);
        }

        public static float GetPlayerJoinLeavePitch()
        {
            return Random.Range(1, 1.2f);
        }
    }
}