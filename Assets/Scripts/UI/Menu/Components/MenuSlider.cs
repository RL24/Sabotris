using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sabotris.UI.Menu
{
    public class MenuSlider : MenuButton
    {
        public event EventHandler<float> OnValueChanged;

        public Slider slider;
        public TMP_Text sliderValueText;

        protected override void Update()
        {
            base.Update();

            sliderValueText.text = $"{Mathf.RoundToInt(slider.value)}";
        }

        public override void NavigateHorizontal(float val)
        {
            SetValue(slider.value + val);
            OnSliderValueChanged();
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