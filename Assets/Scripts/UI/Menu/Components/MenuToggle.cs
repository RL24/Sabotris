using System;
using Sabotris;
using Sabotris.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuToggle : MenuButton
    {
        public event EventHandler<bool> OnValueChanged;

        public RawImage checkbox;

        private bool _isToggledOn;
        private float _isToggledOnLerp;

        protected override void Start()
        {
            checkbox.color = new Color(checkbox.color.r, checkbox.color.g, checkbox.color.b, _isToggledOn.Int());
            OnClick += OnButtonClick;
        }

        protected override void Update()
        {
            _isToggledOnLerp += _isToggledOnLerp.Lerp(_isToggledOn.Int(), GameSettings.UIAnimationSpeed);
            checkbox.color = new Color(checkbox.color.r, checkbox.color.g, checkbox.color.b, _isToggledOnLerp);
        }

        public override void NavigateSelect()
        {
            _isToggledOn = !_isToggledOn;
            OnValueChanged?.Invoke(this, _isToggledOn);
        }

        private void OnButtonClick(object sender, EventArgs args)
        {
            NavigateSelect();
        }
    }
}