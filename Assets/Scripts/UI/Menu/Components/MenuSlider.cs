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
        public float sliderValueMultiplier = 1;
        public float offset = 0;

        protected override void Update()
        {
            base.Update();

            sliderValueText.text = $"{((slider.wholeNumbers ? Mathf.RoundToInt(slider.value) : slider.value) + offset) * sliderValueMultiplier}";
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
            OnValueChanged?.Invoke(this, (slider.value + offset) * sliderValueMultiplier);
        }
    }
}