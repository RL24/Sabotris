using System;
using System.Collections;
using UnityEngine;

namespace UI.Menu.Menus
{
    public class MenuJoinGame : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(5, 11, -3.5f);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(55, -51, -7);

        public MenuButton inputIp, inputPassword, buttonConnect, buttonBack;
        
        public Menu menuMain, menuLobby;

        private string _ip, _password;

        protected override void Start()
        {
            base.Start();
            
            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            if (inputIp is MenuInput miIp)
                miIp.OnValueChanged += OnIpValueChanged;
            
            if (inputPassword is MenuInput miPassword)
                miPassword.OnValueChanged += OnPasswordValueChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;

            if (inputPassword is MenuInput miPassword)
                miPassword.OnValueChanged -= OnPasswordValueChanged;
        }

        private void OnIpValueChanged(object sender, string args)
        {
            _ip = args;
            buttonConnect.isDisabled = _ip.Length == 0 || _password.Length == 0;
        }

        private void OnPasswordValueChanged(object sender, string args)
        {
            _password = args;
            buttonConnect.isDisabled = _ip.Length == 0 || _password.Length == 0;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonConnect))
                StartCoroutine(StartClient());
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        private IEnumerator StartClient()
        {
            SetButtonsDisabled();

            if (!(inputIp is MenuInput miIp))
                yield break;
            
            if (!(inputPassword is MenuInput miPassword))
                yield break;

            var ip = miIp.inputField.text;
            var password = miPassword.inputField.text;
            yield return networkController.Client.StartClient(ip, 47320, password);
            if (networkController.Client.IsConnected)
                menuController.OpenMenu(menuLobby);
            else
                SetButtonsDisabled(false);
        }

        protected override Menu GetBackMenu()
        {
            return menuMain;
        }

        public override Vector3 GetCameraPosition()
        {
            return _cameraPosition;
        }
        
        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }
    }
}