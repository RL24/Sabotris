using System;
using System.Diagnostics;
using System.Linq;
using Sabotris.Network;
using Sabotris.Powers;
using Sabotris.Util;
using Sabotris.Worlds;
using Unity.Mathematics;
using UnityEngine;

namespace Sabotris.Game
{
    public class ContainerSelectorController : MonoBehaviour
    {
        public const int PowerUpUseTimeoutSeconds = 5;
        
        public CameraController cameraController;
        public NetworkController networkController;
         
        public PowerUp PowerUp;
        public Container activatingContainer;
        public Action<Container> SelectedContainerFunc;
        public Container[] exclusions;

        public Vector3 position;
        public Quaternion rotation;
        public Container selectedContainer;
        public Stopwatch PowerUpTimer;

        private bool _active;

        private const int SelectDelay = 500;
        private readonly Stopwatch _selectTimer = new Stopwatch();
        private bool _selectDelayed;
        
        private int _selectedIndex;
        private Container[] _selectableContainers = new Container[0];
        private bool _doneSelecting;

        private void Update()
        {
            if (!_selectableContainers.Any() || !Active)
                return;
            
            var changeContainer = InputUtil.GetChangeContainerSelection();
            if (changeContainer != 0 && !_selectDelayed && !_doneSelecting)
            {
                _selectedIndex += changeContainer;
                _selectDelayed = true;
                _selectTimer.Restart();
            }
            else if (changeContainer == 0)
            {
                _selectDelayed = false;
                _selectTimer.Restart();
            }

            if (_selectTimer.ElapsedMilliseconds > SelectDelay)
            {
                _selectDelayed = false;
                _selectTimer.Restart();
            }

            var container = _selectableContainers[(int) Mathf.Repeat(_selectedIndex, _selectableContainers.Length)];
            if (!container)
                return;

            position = container.rawPosition + (Vector3.back * 20) + (Vector3.up * 22);
            rotation = Quaternion.Euler(30, 0, 0);

            if (!(InputUtil.GetSelectContainer() || PowerUpTimer.ElapsedMilliseconds > (networkController.Client?.LobbyData.PowerUpAutoPickDelay ?? PowerUpUseTimeoutSeconds) * 1000) || _doneSelecting)
                return;

            selectedContainer = container;
            _doneSelecting = true;
            SelectedContainerFunc?.Invoke(container);
        }

        public void End()
        {
            Active = false;
            selectedContainer = null;
            _doneSelecting = false;
        }

        public bool Active
        {
            get => _active;
            set
            {
                if (_active == value)
                    return;

                _active = value;

                if (!_active)
                    return;

                _selectedIndex = 0;
                _selectableContainers = FindObjectsOfType<Container>().Where((container) => exclusions.All(excluded => true/*excluded.id != container.id*/)).OrderBy((container) => container.rawPosition.x).ToArray();
                _selectTimer.Start();
            }
        }
    }
}