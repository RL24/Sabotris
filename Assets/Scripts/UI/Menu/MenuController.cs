﻿using Sabotris;
using Sabotris.Network;
using Sabotris.Util;
using UI.Menu.Menus;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuController : MonoBehaviour
    {
        public NetworkController networkController;
        public World world;
        
        public Volume volume;
        public RawImage background;
        
        public Menu currentMenu,
            nextMenu; // prefab

        private void Update()
        {
            if (currentMenu != null && currentMenu.canvasGroup.alpha.Same(0))
            {
                Destroy(currentMenu.gameObject);
                currentMenu = null;
            }
            
            if (currentMenu == null && nextMenu != null)
            {
                currentMenu = nextMenu;
                currentMenu.canvasGroup.alpha = 0;
                currentMenu.Open = true;
                nextMenu = null;
            }
        }

        public void OpenMenu(Menu prefab)
        {
            if (currentMenu != null)
            {
                if (!currentMenu.Closing)
                    currentMenu.Closing = true;
            }

            var getDof = volume.profile.TryGet(out DepthOfField dof);
            if (prefab == null)
            {
                if (getDof)
                    dof.active = false;
                background.enabled = false;
                return;
            }
            
            if (getDof && !(prefab is MenuGameOver))
                dof.active = true;
            background.enabled = true;
            
            nextMenu = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            nextMenu.menuController = this;
            nextMenu.networkController = networkController;
            nextMenu.world = world;
        }

        public bool IsInMenu => currentMenu != null;

        public Menu GetTargetMenu() => nextMenu != null ? nextMenu : currentMenu;
    }
}