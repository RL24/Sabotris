using System.Collections;
using Sabotris.IO;
using Sabotris.Util;
using TMPro;
using UnityEngine;

namespace Sabotris.UI.Menu
{
    public class MenuCountdown : MonoBehaviour
    {
        private const float Saturation = 0.7f;
        private static readonly Color ColorStart = new Color(1, Saturation, Saturation, 1);
        private static readonly Color ColorMid = new Color(1, 1, Saturation, 1);
        private static readonly Color ColorEnd = new Color(Saturation, 1, Saturation, 1);
        
        public TMP_Text text;
        
        private Color _color = Color.clear;
        private Vector3 _size = Vector3.zero;

        private void Start()
        {
            transform.localScale = _size;
        }

        private void Update()
        {
            if (!text)
                return;

            text.color = Color.Lerp(text.color, _color, GameSettings.Settings.uiAnimationSpeed.Delta());
            transform.localScale = Vector3.Lerp(transform.localScale, _size, GameSettings.Settings.uiAnimationSpeed.Delta());
        }

        public IEnumerator StartCountdown()
        {
            _color = ColorStart;
            _size = Vector3.one;
            text.text = "3";
            yield return new WaitForSeconds(0.8f);
            _size = Vector3.zero;
            yield return new WaitForSeconds(0.2f);

            _color = ColorMid;
            _size = Vector3.one;
            text.text = "2";
            yield return new WaitForSeconds(0.8f);
            _size = Vector3.zero;
            yield return new WaitForSeconds(0.2f);
            
            _color = ColorEnd;
            _size = Vector3.one;
            text.text = "1";
            yield return new WaitForSeconds(0.8f);
            _size = Vector3.zero;
            yield return new WaitForSeconds(0.2f);
        }

        public void StopCountdown()
        {
            _color = Color.clear;
            _size = Vector3.zero;
        }
    }
}