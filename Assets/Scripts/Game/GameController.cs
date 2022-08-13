using System;
using Sabotris.Achievements;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
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
        public NetworkController networkController;
        public MenuController menuController;
        public AchievementController achievementController;
        public World world;

        public Menu menuLobby;

        public UniversalRendererData forwardRendererData;
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

            Callback<GameLobbyJoinRequested_t>.Create(LobbyJoinRequestReceived);
            
            networkController.Client?.RegisterListener(this);
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
        
        private void LobbyJoinRequestReceived(GameLobbyJoinRequested_t joinRequest)
        {
            if (networkController.Client == null || networkController.Client?.IsConnected == true)
                return;
            
            void ConnectedToServer(object sender, HSteamNetConnection? connection)
            {
                if (networkController.Client != null)
                {
                    networkController.Client.OnConnectedToServerEvent -= ConnectedToServer;
                    networkController.Client.OnFailedToConnectToServerEvent -= FailedToConnect;
                }

                if (connection != null)
                    menuController.OpenMenu(menuLobby);
            }

            void FailedToConnect(object sender, EventArgs args)
            {
                if (networkController.Client == null)
                    return;
                
                networkController.Client.OnConnectedToServerEvent -= ConnectedToServer;
                networkController.Client.OnFailedToConnectToServerEvent -= FailedToConnect;
            }

            networkController.Client.OnConnectedToServerEvent += ConnectedToServer;
            networkController.Client.OnFailedToConnectToServerEvent += FailedToConnect;

            networkController.Client?.JoinLobby(joinRequest.m_steamIDLobby);
        }

        [PacketListener(PacketTypeId.GameStart, PacketDirection.Client)]
        public void OnGameStart(PacketGameStart packet)
        {
            if (networkController.Server?.Running == true)
                achievementController.Achieve(Achievement.HostAMatch);
        }
    }
}