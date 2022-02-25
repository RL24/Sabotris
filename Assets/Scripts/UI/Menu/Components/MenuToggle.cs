using System;
using Sabotris.IO;
using Sabotris.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Sabotris.UI.Menu
{
    public class MenuToggle : MenuButton
    {
        public event EventHandler<bool> OnValueChanged;

        public RawImage checkbox;

        public bool isToggledOn;
        private float _isToggledOnLerp;

        protected override void Start()
        {
            checkbox.color = new Color(checkbox.color.r, checkbox.color.g, checkbox.color.b, isToggledOn.Int());
            OnClick += OnButtonClick;
        }

        protected override void Update()
        {
            _isToggledOnLerp += _isToggledOnLerp.Lerp(isToggledOn.Int(), GameSettings.Settings.UIAnimationSpeed);
            checkbox.color = new Color(checkbox.color.r, checkbox.color.g, checkbox.color.b, _isToggledOnLerp);

            base.Update();
        }

        public override void NavigateSelect()
        {
            isToggledOn = !isToggledOn;
            OnValueChanged?.Invoke(this, isToggledOn);
        }

        private void OnButtonClick(object sender, EventArgs args)
        {
            NavigateSelect();
        }
    }
}