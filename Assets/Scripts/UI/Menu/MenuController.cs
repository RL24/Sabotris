using Sabotris.Network;
using Sabotris.UI.Menu.Menus;
using Sabotris.Util;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Sabotris.UI.Menu
{
    public class MenuController : MonoBehaviour
    {
        public NetworkController networkController;
        public CameraController cameraController;
        public World world;

        public Volume volume;
        public RawImage background;

        public Menu currentMenu,
            nextMenu;

        public GameObject interactionBlocker;

        public DepthOfField dof;

        private void Start()
        {
            volume.profile.TryGet(out dof);
        }
        
        private void Update()
        {
            if (currentMenu && currentMenu.Closing && currentMenu.canvasGroup.alpha.Same(0))
            {
                Destroy(currentMenu.gameObject);
                currentMenu = null;
            }

            if (!currentMenu && nextMenu)
            {
                currentMenu = nextMenu;
                currentMenu.canvasGroup.alpha = 0;
                currentMenu.Open = true;
                nextMenu = null;
            }
        }

        public void OpenMenu(Menu prefab)
        {
            if (currentMenu && !currentMenu.Closing)
                currentMenu.Closing = true;

            if (!prefab)
            {
                if (dof)
                    dof.active = false;
                background.enabled = false;
                return;
            }

            if (dof && !(prefab is MenuGameOver))
                dof.active = true;
            else if (prefab is MenuGameOver)
                dof.active = false;
            background.enabled = true;

            nextMenu = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            nextMenu.name = $"Menu-{prefab.name}";
            nextMenu.menuController = this;
            nextMenu.networkController = networkController;
            nextMenu.cameraController = cameraController;
            nextMenu.world = world;
        }

        public void PreventInteractions(bool prevent)
        {
            interactionBlocker.SetActive(prevent);    
        }

        public bool IsInMenu => currentMenu;

        public Menu GetTargetMenu() => nextMenu ? nextMenu : currentMenu;
    }
}