using System;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace Sabotris.UI.Menu
{
    public class MenuBind : MenuButton
    {
        public event EventHandler<KeyControl> OnKeyBindChangedEvent;
        public event EventHandler<ButtonControl> OnGamepadBindChangedEvent;

        public TMP_Text label, valueLabel;
        public bool isGamepad;

        public string OriginalText { get; set; }

        private string _valueText;

        public string ValueText
        {
            get => _valueText;
            set
            {
                if (_valueText == value)
                    return;

                _valueText = value;

                if (valueLabel)
                    valueLabel.text = _valueText;
            }
        }

        protected override void Start()
        {
            base.Start();

            OriginalText = label.text;
        }

        public override void NavigateSelect()
        {
            if (!menu.Interactable)
                return;

            menu.Interactable = false;
            label.text = "Press [ANY] Key/Button";

            InputSystem.onEvent
                .ForDevice<Keyboard>()
                .Where((e) => e.HasButtonPress())
                .CallOnce(OnKeyboardInputEvent);

            if (isGamepad)
                InputSystem.onEvent
                    .ForDevice<Gamepad>()
                    .Where((e) => e.HasButtonPress())
                    .CallOnce(OnGamepadInputEvent);
        }

        private void OnKeyboardInputEvent(InputEventPtr eventPtr)
        {
            if (menu.Interactable)
                return;

            menu.Interactable = true;
            label.text = OriginalText;

            var control = eventPtr.EnumerateChangedControls().OfType<KeyControl>().LastOrDefault();
            if (control == null)
            {
                if (!isGamepad)
                    OnKeyBindChangedEvent?.Invoke(this, null);
                return;
            }

            switch (control.keyCode)
            {
                case Key.Escape:
                    return;
                case Key.Delete:
                    OnKeyBindChangedEvent?.Invoke(this, null);
                    return;
            }

            OnKeyBindChangedEvent?.Invoke(this, control);
        }

        private void OnGamepadInputEvent(InputEventPtr eventPtr)
        {
            if (menu.Interactable)
                return;

            menu.Interactable = true;
            label.text = OriginalText;

            var control = eventPtr.EnumerateChangedControls().OfType<ButtonControl>().LastOrDefault();
            if (control == null)
            {
                OnGamepadBindChangedEvent?.Invoke(this, null);
                return;
            }

            OnGamepadBindChangedEvent?.Invoke(this, control);
        }
    }
}