using System;
using Sabotris;
using Sabotris.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuToggle : MenuButton
    {
        public RawImage checkbox;

        public bool isToggledOn;
        private float _isToggledOnLerp;

        protected override void Start()
        {
            checkbox.color = new Color(checkbox.color.r, checkbox.color.g, checkbox.color.b, isToggledOn.Int());
        }

        protected override void Update()
        {
            _isToggledOnLerp += _isToggledOnLerp.Lerp(isToggledOn.Int(), GameSettings.UIAnimationSpeed);
            checkbox.color = new Color(checkbox.color.r, checkbox.color.g, checkbox.color.b, _isToggledOnLerp);
        }

    }
}