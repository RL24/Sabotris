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
            music.volume = 0.25f * (GameSettings.Settings.MasterVolume * 0.01f);
        }
    }
}