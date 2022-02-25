using System;
using TMPro;
using UnityEngine;

namespace Sabotris.UI.Menu
{
    public class MenuCarousel : MenuButton
    {
        public event EventHandler<int> OnValueChanged;

        public MenuButton previous, next;
        public TMP_Text value;
        public string[] values;
        public int index;

        protected override void Start()
        {
            previous.OnClick += OnPreviousClick;
            next.OnClick += OnNextClick;

            value.text = values[index];
        }

        public override void NavigateHorizontal(float val)
        {
            index = (int) Mathf.Repeat(index + val, values.Length);
            value.text = values[index];
            OnValueChanged?.Invoke(this, index);
        }

        private void OnPreviousClick(object sender, EventArgs args)
        {
            NavigateHorizontal(-1);
        }

        private void OnNextClick(object sender, EventArgs args)
        {
            NavigateHorizontal(1);
        }
    }
}