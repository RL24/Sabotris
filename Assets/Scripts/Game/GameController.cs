using System;
using Sabotris.IO;
using Sabotris.UI.Menu;
using Steamworks;
using Translations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Sabotris
{
    public class GameController : MonoBehaviour
    {
        public MenuController menuController;
        
        public ForwardRendererData forwardRendererData;
        public Volume renderVolume;
        
        private ScriptableRendererFeature _renderFeatureSsao;
        private DepthOfField _dof;

        public Container ControllingContainer { get; set; }

        private void Start()
        {
            _renderFeatureSsao = forwardRendererData.rendererFeatures.Find((feature) => feature.name.Equals("NewScreenSpaceAmbientOcclusion"));
            renderVolume.profile.TryGet(out _dof);
            
            SteamNetworkingUtils.InitRelayNetworkAccess();

            GameSettings.OnBeforeSaveEvent += OnBeforeSave;
            GameSettings.OnAfterLoadEvent += OnAfterLoad;
            
            GameSettings.Load();
            GameSettings.Save();
        }

        private void Update()
        {
            var lockState = CursorLockMode.Locked;
            var visible = false;
            if (menuController.IsInMenu)
            {
                lockState = CursorLockMode.None;
                visible = true;
            }

            if (Cursor.lockState != lockState)
                Cursor.lockState = lockState;

            if (Cursor.visible != visible)
                Cursor.visible = visible;
        }

        private void OnBeforeSave(object sender, EventArgs e)
        {
            UpdateFromSettings();
        }

        private void OnAfterLoad(object sender, EventArgs e)
        {
            UpdateFromSettings();
        }

        private void UpdateFromSettings()
        {
            _renderFeatureSsao.SetActive(GameSettings.Settings.AmbientOcclusion);
            _dof.mode.value = GameSettings.Settings.MenuDofMode;
            Screen.fullScreenMode = GameSettings.Settings.FullscreenMode;
            Localization.CurrentLocale = GameSettings.Settings.Language;
        }
    }
}