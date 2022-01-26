using System;
using Sabotris;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu
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
        }
        
        protected override void Update()
        {
            base.Update();
            
            Value = inputField.text;
            background.color = Color.Lerp(background.color, inputField.isFocused ? SelectedColor : _startBackgroundColor,
                GameSettings.UIAnimationSpeed);
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