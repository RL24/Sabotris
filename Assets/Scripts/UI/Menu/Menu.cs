﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sabotris.Audio;
using Sabotris.Game;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Util;
using Sabotris.Util.Input;
using Sabotris.Worlds;
using UnityEngine;

namespace Sabotris.UI.Menu
{
    public abstract class Menu : MonoBehaviour
    {
        public InputController inputController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public AudioController audioController;
        public World world;

        public CanvasGroup canvasGroup;
        public List<MenuButton> buttons;

        private int _lastSelectedButton,
            _selectedButton = -1;

        private readonly Stopwatch _selectTimer = new Stopwatch();
        private readonly Stopwatch _sideTimer = new Stopwatch();

        private const int SelectDelayMsMin = 20;
        private const int SelectDelayMsMax = 250;
        private const int SideDelayMsMin = 20;
        private const int SideDelayMsMax = 250;

        private int _selectDelayMs = 250;
        private int _sideDelayMs = 250;

        public bool Open { get; set; }
        public bool Closing { get; set; }

        private bool _interactable = true;

        public bool Interactable
        {
            get => _interactable;
            set
            {
                if (_interactable == value)
                    return;

                _interactable = value;

                menuController.PreventInteractions(!Interactable);
            }
        }

        protected virtual void Start()
        {
            canvasGroup.alpha = 0;

            foreach (var menuButton in buttons)
            {
                menuButton.OnMouseEnter += OnMouseEnterButton;
                menuButton.OnMouseExit += OnMouseExitButton;

                if (menuButton.isSelected)
                    SelectedButton = buttons.FindIndex((button) => button == menuButton);
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (var menuButton in buttons)
            {
                menuButton.OnMouseEnter -= OnMouseEnterButton;
                menuButton.OnMouseExit -= OnMouseExitButton;
            }
        }

        private void Update()
        {
            if (!Open)
                return;

            var navigate = Mathf.RoundToInt(inputController.GetMoveUINavigateVertical()) * Interactable.Int();
            var navigateHor = Mathf.RoundToInt(inputController.GetMoveUINavigateHorizontal()) * Interactable.Int();

            if (_selectTimer.ElapsedMilliseconds > _selectDelayMs || navigate == 0)
            {
                _selectTimer.Reset();
                _selectDelayMs = navigate != 0 ? Mathf.Clamp(_selectDelayMs - 20, SelectDelayMsMin, SelectDelayMsMax) : SelectDelayMsMax;
            }

            if (_sideTimer.ElapsedMilliseconds > _sideDelayMs || navigateHor == 0)
            {
                _sideTimer.Reset();
                _sideDelayMs = navigateHor != 0 ? Mathf.Clamp(_sideDelayMs - 40, SideDelayMsMin, SideDelayMsMax) : SideDelayMsMax;
            }

            if (navigate != 0 && !_selectTimer.IsRunning)
            {
                _selectTimer.Start();

                if (SelectedButton == -1)
                    SelectedButton = _lastSelectedButton;
                else
                    SelectedButton = (int) Mathf.Repeat(SelectedButton + navigate, buttons.Count);
            }

            if (navigateHor != 0 && !_sideTimer.IsRunning)
            {
                _sideTimer.Start();

                if (SelectedButton != -1)
                    buttons[SelectedButton].NavigateHorizontal(navigateHor);
            }

            canvasGroup.alpha += canvasGroup.alpha.Lerp((!Closing).Int(), GameSettings.Settings.menuCameraSpeed.Delta() * 2);

            if (Interactable)
            {
                if (inputController.GetUISelect())
                {
                    if (_selectedButton != -1)
                        buttons[SelectedButton].NavigateSelect();
                    else SelectedButton = _lastSelectedButton;
                }
                else if (inputController.GetUIBack())
                    GoBack();
            }
        }

        protected virtual Menu GetBackMenu() => null;

        protected virtual void GoBack()
        {
            GameSettings.Load();

            SetButtonsDisabled();

            menuController.OpenMenu(GetBackMenu());
        }

        protected void SetButtonsDisabled(bool disabled = true)
        {
            foreach (var menuButton in buttons)
                menuButton.isDisabled = disabled;
        }

        protected void OnMouseEnterButton(object sender, EventArgs args)
        {
            if (!Open || !Interactable)
                return;

            SelectedButton = _lastSelectedButton = buttons.IndexOf(sender as MenuButton);
        }

        protected void OnMouseExitButton(object sender, EventArgs args)
        {
            if (!Open || !Interactable || SelectedButton < 0 || SelectedButton >= buttons.Count)
                return;

            var selectedButton = buttons[SelectedButton];
            selectedButton.isSelected = false;
            SelectedButton = -1;
        }

        public abstract Vector3 GetCameraPosition();
        public abstract Quaternion GetCameraRotation();

        protected int SelectedButton
        {
            get => _selectedButton;
            set
            {
                if (SelectedButton == value)
                    return;

                if (SelectedButton != -1)
                    buttons[SelectedButton].isSelected = false;

                _selectedButton = value;

                if (SelectedButton != -1)
                {
                    buttons[SelectedButton].isSelected = true;

                    if (!audioController)
                        return;

                    audioController.hoverButton.PlayModifiedSound(AudioController.GetButtonHoverVolume(), AudioController.GetButtonHoverPitch());
                }
            }
        }
    }
}