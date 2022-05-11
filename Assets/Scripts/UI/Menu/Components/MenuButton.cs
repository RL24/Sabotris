using System;
using Sabotris.Audio;
using Sabotris.IO;
using Sabotris.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Sabotris.UI.Menu
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly Vector3 HoverPosition = new Vector3(40, 0, 0);
        private static readonly Color ColorDisabled = new Color(1, 1, 1, 0.3f);
        private static readonly Color ColorActive = new Color(0.6f, 0.9f, 1, 1f);

        public event EventHandler OnMouseEnter, OnMouseExit, OnClick;

        public Menu menu;
        public RectTransform rectTransform;
        public TMP_Text text;

        public bool isHovered,
            isSelected,
            isDisabled;

        private Vector3 _startPosition;
        private Color _startColor = Color.white;

        protected virtual void Start()
        {
            if (rectTransform)
                _startPosition = rectTransform.localPosition;

            if (text)
                _startColor = text.color;
        }

        protected virtual void Update()
        {
            if (!text)
                return;

            var color = isDisabled
                ? ColorDisabled
                : isHovered || isSelected
                    ? ColorActive
                    : _startColor;
            text.color = Color.Lerp(text.color, color, GameSettings.Settings.uiAnimationSpeed.Delta());
        }

        private void FixedUpdate()
        {
            if (rectTransform)
                rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, (isHovered || isSelected) && !isDisabled ? HoverPosition + _startPosition : _startPosition, GameSettings.Settings.uiAnimationSpeed.FixedDelta());
        }

        public virtual void NavigateSelect()
        {
            if (isDisabled)
                return;
            OnClick?.Invoke(this, null);
        }

        public virtual void NavigateHorizontal(float val)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!menu.Interactable || isDisabled)
                return;
            
            if (menu.audioController != null)
                menu.audioController.clickButton.PlayModifiedSound(AudioController.GetButtonClickVolume(), AudioController.GetButtonClickPitch());
            
            NavigateSelect();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!menu.Interactable || isDisabled)
                return;
            OnMouseEnter?.Invoke(this, null);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!menu.Interactable)
                return;
            OnMouseExit?.Invoke(this, null);
        }
    }
}