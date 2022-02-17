using Sabotris.Network;
using Sabotris.Util;
using Steamworks;
using UI.Menu;
using UnityEngine;

namespace Sabotris
{
    public class GameController : MonoBehaviour
    {
        public const string AppIdentifier = "sabotris";

        public MenuController menuController;
        public NetworkController networkController;

        public Container ControllingContainer { get; set; }

        private void Start()
        {
            SteamNetworkingUtils.InitRelayNetworkAccess();
            
            networkController.Server.OnServerStartEvent += (_, __) =>
            {
                Logging.Log(true, "Server started, now listening for for connections");
            };

            networkController.Server.OnServerStopEvent += (_, reason) =>
            {
                Logging.Log(true, "Server stopped: {0}", reason);
            };
            
            networkController.Client.OnConnectedToServerEvent += (_, reason) =>
            {
                Logging.Log(false, "Connected to server");
            };

            networkController.Client.OnDisconnectedFromServerEvent += (_, reason) =>
            {
                Logging.Log(false, "Disconnected from server: {0}", reason);
            };
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
