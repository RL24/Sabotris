using System;
using Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using UnityEngine;

namespace UI.Menu.Menus
{
    public class MenuLobby : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(-3, 8, -17);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(34, 34, 5);

        public MenuButton buttonStartGame, buttonBack;
        
        public Menu menuHost, menuJoin;

        protected override void Start()
        {
            base.Start();
            
            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;
            
            networkController.Client.RegisterListener(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
            
            networkController.Client.DeregisterListener(this);
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonStartGame))
                networkController.Client?.SendPacket(new PacketGameStart());
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        protected override void GoBack()
        {
            base.GoBack();
            networkController.Client.DisconnectSocket(DisconnectReason.ClientDisconnected);
            networkController.Server.DisconnectSockets(DisconnectReason.ServerClosed);
        }

        protected override Menu GetBackMenu()
        {
            return networkController.Server.Running ? menuHost : menuJoin;
        }

        public override Vector3 GetCameraPosition()
        {
            return _cameraPosition;
        }
        
        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnGameStart(PacketGameStart packet)
        {
            menuController.OpenMenu(null);
        }
    }
}