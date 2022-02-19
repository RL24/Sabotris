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
        
        public event EventHandler<string> OnValueChanged;

        public RawImage background;
        public TMP_InputField inputField;
        private string _value;

        private Color _startBackgroundColor;

        protected override void Start()
        {
            base.Start();

            if (background != null)
                _startBackgroundColor = background.color;

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

        public string Value
        {
            get => _value;
            set
            {
                if (value == _value)
                    return;
                
                _value = value;
                
                OnValueChanged?.Invoke(this, Value);
            }
        }

    }
}