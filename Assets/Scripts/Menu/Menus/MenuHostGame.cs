﻿using System;
using System.Collections;
using UnityEngine;

namespace Menu.Menus
{
    public class MenuHostGame : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(5, 11, -3.5f);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(55, -51, -7);

        public MenuButton inputPassword, buttonJoinLobby, buttonBack;
        
        public Menu menuMain, menuLobby;

        protected override void Start()
        {
            base.Start();
            
            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

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

        private void OnPasswordValueChanged(object sender, string args)
        {
            buttonJoinLobby.isDisabled = args.Length == 0;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonJoinLobby))
                StartCoroutine(StartServer());
            else if (sender.Equals(buttonBack))
                GoBack();
        }

        private IEnumerator StartServer()
        {
            SetButtonsState();

            if (!(inputPassword is MenuInput miPassword))
                yield break;
            
            var password = miPassword.inputField.text;
            yield return networkController.Server.StartServer(password, 47320);
            yield return new WaitForSeconds(0.5f);
            yield return networkController.Client.StartClient("127.0.0.1", 47320, password);
            yield return new WaitForSeconds(0.5f);
            if (networkController.Client.IsConnected)
                menuController.OpenMenu(menuLobby);
            else
                SetButtonsState(false);
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