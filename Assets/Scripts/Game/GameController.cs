using Steamworks;
using UI.Menu;
using UnityEngine;

namespace Sabotris
{
    public class GameController : MonoBehaviour
    {
        public MenuController menuController;

        public Container ControllingContainer { get; set; }

        private void Start()
        {
            SteamNetworkingUtils.InitRelayNetworkAccess();
        }

        private void Update()
        {
            var lockState = CursorLockMode.Locked;
            var visible = false;
            if (menuController.IsInMenu)
            {
                lockState = CursorLockMode.None;
                visible = true;
            }

            if (Cursor.lockState != lockState)
                Cursor.lockState = lockState;

            if (Cursor.visible != visible)
                Cursor.visible = visible;
        }
    }
}