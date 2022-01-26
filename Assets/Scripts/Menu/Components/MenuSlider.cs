using System;
using Sabotris;
using Sabotris.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuSlider : MenuButton
    {
        public event EventHandler<float> OnValueChanged;

        public Slider slider;
        
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