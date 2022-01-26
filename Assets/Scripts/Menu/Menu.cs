using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sabotris;
using Sabotris.Network;
using Sabotris.Util;
using Steamworks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Menu
{
    public abstract class Menu : MonoBehaviour
    {
        public MenuController menuController;
        public NetworkController networkController;
        public CanvasGroup canvasGroup;
        public List<MenuButton> buttons;

        private int _lastSelectedButton,
                    _selectedButton = -1;

        private readonly Stopwatch _selectTimer = new Stopwatch();
        public int SelectDelayMs { get; set; } = 250;

        public bool Open { get; set; }
        public bool Closing { get; set; }
        
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
            
            var navigate = Mathf.RoundToInt(InputUtil.GetMoveUINavigate());
            
            if (_selectTimer.ElapsedMilliseconds > SelectDelayMs || navigate == 0)
                _selectTimer.Reset();

            if (navigate != 0 && !_selectTimer.IsRunning)
            {
                _selectTimer.Start();

                if (SelectedButton == -1)
                    SelectedButton = _lastSelectedButton;
                else
                    SelectedButton = (int) Mathf.Repeat(SelectedButton + navigate, buttons.Count);
            }
            
            canvasGroup.alpha += canvasGroup.alpha.Lerp((!Closing).Int(), GameSettings.MenuCameraSpeed * 2);

            if (InputUtil.GetUISelect())
            {
                if (_selectedButton != -1)
                    buttons[_selectedButton].OnPointerClick(null);
                else _selectedButton = _lastSelectedButton;
            } else if (InputUtil.GetUIBack())
                GoBack();
        }

        protected virtual Menu GetBackMenu() => null;
        
        protected virtual void GoBack()
        {
            SetButtonsState();
            
            menuController.OpenMenu(GetBackMenu());
        }

        protected virtual void SetButtonsState(bool disabled = true)
        {
            foreach (var menuButton in buttons)
                menuButton.isDisabled = disabled;
        }

        private void OnMouseEnterButton(object sender, EventArgs args)
        {
            if (!Open)
                return;
            
            SelectedButton = _lastSelectedButton = buttons.IndexOf(sender as MenuButton);
        }

        private void OnMouseExitButton(object sender, EventArgs args)
        {
            if (!Open)
                return;
            
            if (SelectedButton < 0 || SelectedButton >= buttons.Count)
                return;
            var selectedButton = buttons[SelectedButton];
            selectedButton.isSelected = false;
            SelectedButton = -1;
        }
        
        public abstract Vector3 GetCameraPosition();
        public abstract Quaternion GetCameraRotation();

        private int SelectedButton
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
                    buttons[SelectedButton].isSelected = true;
            }
        }
    }
}