using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UI.Menu.Menus
{
    public class MenuSettingsVideo : Menu
    {
        private readonly Vector3 _cameraPosition = new Vector3(3, 6, 8);
        private readonly Quaternion _cameraRotation = Quaternion.Euler(21, 209, 5);

        public ForwardRendererData forwardRendererData;

        private ScriptableRendererFeature _renderFeatureSsao;

        public MenuButton buttonSsao,
            buttonMenuDof,
            buttonFullscreen,
            buttonBack;

        public Menu menuSettings;

        protected override void Start()
        {
            base.Start();

            _renderFeatureSsao = forwardRendererData.rendererFeatures.Find((feature) => feature.name.Equals("NewScreenSpaceAmbientOcclusion"));
            
            foreach (var menuButton in buttons)
                menuButton.OnClick += OnClickButton;

            if (buttonSsao is MenuToggle toggleSsao)
            {
                toggleSsao.isToggledOn = _renderFeatureSsao is {isActive: true};
                toggleSsao.OnValueChanged += OnSsaoToggled;
            }
            
            if (buttonMenuDof is MenuCarousel carouselMenuDof)
            {
                switch (menuController.dof.mode.value)
                {
                    case DepthOfFieldMode.Off:
                        carouselMenuDof.index = 0;
                        break;
                    case DepthOfFieldMode.Gaussian:
                        carouselMenuDof.index = 1;
                        break;
                    case DepthOfFieldMode.Bokeh:
                        carouselMenuDof.index = 2;
                        break;
                }

                carouselMenuDof.OnValueChanged += OnMenuDofModeChanged;
            }

            if (buttonFullscreen is MenuCarousel carouselFullscreen)
            {
                switch (Screen.fullScreenMode)
                {
                    case FullScreenMode.FullScreenWindow:
                        carouselFullscreen.index = 0;
                        break;
                    case FullScreenMode.ExclusiveFullScreen:
                        carouselFullscreen.index = 1;
                        break;
                    default:
                        carouselFullscreen.index = 2;
                        break;
                }

                carouselFullscreen.OnValueChanged += OnFullscreenModeChanged;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuButton in buttons)
                menuButton.OnClick -= OnClickButton;
        }

        private void OnClickButton(object sender, EventArgs args)
        {
            if (!Open)
                return;

            if (sender.Equals(buttonBack))
                GoBack();
        }

        private void OnSsaoToggled(object sender, bool active)
        {
            if (_renderFeatureSsao)
                _renderFeatureSsao.SetActive(active);
        }

        private void OnMenuDofModeChanged(object sender, int index)
        {
            switch (index)
            {
                case 0:
                    menuController.dof.mode.value = DepthOfFieldMode.Off;
                    break;
                case 1:
                    menuController.dof.mode.value = DepthOfFieldMode.Gaussian;
                    break;
                case 2:
                    menuController.dof.mode.value = DepthOfFieldMode.Bokeh;
                    break;
            }
        }

        private void OnFullscreenModeChanged(object sender, int index)
        {
            switch (index)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }

        protected override Menu GetBackMenu()
        {
            return menuSettings;
        }

        public override Vector3 GetCameraPosition()
        {
            return _cameraPosition;
        }

        public override Quaternion GetCameraRotation()
        {
            return _cameraRotation;
        }
    }
}