using System;
using Sabotris.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sabotris.UI.Menu
{
    public class MenuInput : MenuButton
    {
        private static readonly Color SelectedColor = new Color(1, 1, 1, 0.2f);

        public event EventHandler<string> OnValueChangedEvent;
        public event EventHandler<string> OnSubmitEvent;

        public RawImage background;
        public TMP_InputField inputField;
        private string _value;

        private Color _startBackgroundColor;

        protected override void Start()
        {
            base.Start();

            if (background)
                _startBackgroundColor = background.color;
            
            inputField.onSubmit.AddListener((str) =>
            {
                if (!menu.Interactable)
                    return;
                OnSubmitEvent?.Invoke(this, str);
            });

            if (isSelected)
                NavigateSelect();
        }

        protected override void Update()
        {
            base.Update();

            isSelected = inputField.isFocused;

            Value = inputField.text;
            background.color = Color.Lerp(background.color, inputField.isFocused ? SelectedColor : _startBackgroundColor, GameSettings.Settings.UIAnimationSpeed);
        }

        public override void NavigateSelect()
        {
            base.NavigateSelect();

            inputField.Select();
            inputField.ActivateInputField();
        }

        private string Value
        {
            get => _value;
            set
            {
                if (value == _value)
                    return;

                _value = value;

                OnValueChangedEvent?.Invoke(this, Value);
            }
        }
    }
}