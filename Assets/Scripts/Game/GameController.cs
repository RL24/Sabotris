using System;
using Sabotris.IO;
using Sabotris.Translations;
using Sabotris.UI.Menu;
using Sabotris.Worlds;
using Sabotris.Worlds.Tutorial;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Sabotris.Game
{
    public class GameController : MonoBehaviour
    {
        public MenuController menuController;
        public World world;

        public ForwardRendererData forwardRendererData;
        public Volume renderVolume;
        public TutorialHelper tutorialHelper;

        private ScriptableRendererFeature _renderFeatureSsao;
        private DepthOfField _dof;

        public Container ControllingContainer { get; set; }

        public InputActionAsset inputActions;

        private void Start()
        {
            // Util.Random.SetSeed(0); // DEBUG ONLY

            inputActions.Enable();
            
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

            if (!tutorialHelper)
                return;

            var showingTutorial = GameSettings.Settings.tutorial && ControllingContainer && ControllingContainer.ControllingShape;
            tutorialHelper.gameObject.SetActive(showingTutorial);
            tutorialHelper.shape = showingTutorial ? ControllingContainer.ControllingShape : null;
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
            _renderFeatureSsao.SetActive(GameSettings.Settings.ambientOcclusion);
            _dof.mode.value = GameSettings.Settings.menuDofMode;
            Screen.fullScreenMode = GameSettings.Settings.fullscreenMode;
            Localization.CurrentLocale = GameSettings.Settings.language;
        }
    }
}