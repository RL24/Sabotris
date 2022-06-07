using System.Diagnostics;
using Sabotris.Game;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Powers;
using Sabotris.Translations;
using Sabotris.Util;
using TMPro;
using UnityEngine;

namespace Sabotris.UI
{
    public class SelectorOverlay : MonoBehaviour
    {
        public NetworkController networkController;
        
        public CanvasGroup canvasGroup;
        public TMP_Text subLabel;
        public TMP_Text timerLabel;

        private bool _open;
        private Stopwatch _timer;

        private void Start()
        {
            if (subLabel)
                subLabel.text = Localization.Translate(TranslationKey.UiHudSelectContainerSubLabel, "None");

            if (timerLabel)
                timerLabel.text = Localization.Translate(TranslationKey.UiHudSelectContainerTimerLabel, $"{(((networkController.Client?.LobbyData.PowerUpAutoPickDelay ?? ContainerSelectorController.PowerUpUseTimeoutSeconds) * 1000) - _timer?.ElapsedMilliseconds) / 1000.0:F1}");
        }

        private void Update()
        {
            canvasGroup.alpha += canvasGroup.alpha.Lerp(_open.Int(), GameSettings.Settings.uiAnimationSpeed.Delta());

            if (timerLabel)
                timerLabel.text = Localization.Translate(TranslationKey.UiHudSelectContainerTimerLabel, $"{(((networkController.Client?.LobbyData.PowerUpAutoPickDelay ?? ContainerSelectorController.PowerUpUseTimeoutSeconds) * 1000) - _timer?.ElapsedMilliseconds) / 1000.0:F1}");
        }

        public void Open(PowerUp power, Stopwatch timer)
        {
            subLabel.text = Localization.Translate(TranslationKey.UiHudSelectContainerSubLabel, power.ToString());
            _open = true;
            _timer = timer;
        }

        public void Close()
        {
            _open = false;
        }
        
    }
}