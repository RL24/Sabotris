using Sabotris.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sabotris.UI.Menu
{
    public class MenuFadeText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly Color ColorHover = new Color(1, 1, 1, 0.05f);

        public TMP_Text text;

        private bool _isHovered;

        private Color _startColor = Color.white;

        private void Start()
        {
            if (text)
                _startColor = text.color;
        }

        private void Update()
        {
            if (!text)
                return;

            var color = _isHovered
                ? ColorHover
                : _startColor;
            text.color = Color.Lerp(text.color, color, GameSettings.Settings.uiAnimationSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
        }
    }
}