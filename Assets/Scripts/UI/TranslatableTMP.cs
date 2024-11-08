﻿using Sabotris.Translations;
using TMPro;
using UnityEngine;

namespace Sabotris.UI
{
    public class TranslatableTMP : MonoBehaviour
    {
        public TranslationKey key;
        public TMP_Text text;

        private void Start()
        {
            UpdateTranslation();
        }

        public void UpdateTranslation()
        {
            text.text = Localization.Translate(key);
        }
    }
}