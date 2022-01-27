using System;
using Sabotris;
using Sabotris.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuSlider : MenuButton
    {
        public event EventHandler<float> OnValueChanged;

        public Slider slider;

        public override void NavigateHorizontal(float val)
        {
            SetValue(slider.value + val);
        }
        
        public void SetValue(float value)
        {
            slider.SetValueWithoutNotify(value);
        }
        
        public void OnSliderValueChanged()
        {
            OnValueChanged?.Invoke(this, slider.value);
        }
        
    }
}