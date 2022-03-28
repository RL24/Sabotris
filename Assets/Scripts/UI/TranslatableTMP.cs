﻿using TMPro;
using Sabotris.Translations;
using UnityEngine;

namespace Sabotris.UI
{
    public class TranslatableTMP : MonoBehaviour
    {
        public TranslationKey key;
        public TMP_Text text;
        
        private void Start()
        {
            text.text = Localization.Translate(key);
        }
    }
}