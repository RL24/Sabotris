using System;
using Sabotris;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu
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

        private void OnPreviousClick(object sender, EventArgs args)
        {
            index = (int) Mathf.Repeat(index - 1, values.Length);
            value.text = values[index];
            OnValueChanged?.Invoke(this, index);
        }
        
        private void OnNextClick(object sender, EventArgs args)
        {
            index = (int) Mathf.Repeat(index + 1, values.Length);
            value.text = values[index];
            OnValueChanged?.Invoke(this, index);
        }
        
    }
}