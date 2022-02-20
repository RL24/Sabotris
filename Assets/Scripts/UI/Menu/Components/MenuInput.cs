using System;
using Sabotris;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
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

            if (background != null)
                _startBackgroundColor = background.color;
            
            inputField.onSubmit.AddListener((str) => OnSubmitEvent?.Invoke(this, str));

            if (isSelected)
                NavigateSelect();
        }

        protected override void Update()
        {
            base.Update();

            isSelected = inputField.isFocused;

            Value = inputField.text;
            background.color = Color.Lerp(background.color, inputField.isFocused ? SelectedColor : _startBackgroundColor,
                GameSettings.UIAnimationSpeed);
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