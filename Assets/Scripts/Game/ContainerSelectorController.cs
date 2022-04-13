using System;
using System.Diagnostics;
using System.Linq;
using Sabotris.Util;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Sabotris
{
    public class ContainerSelectorController : MonoBehaviour
    {
        public CameraController cameraController;
        public Container activatingContainer;
        public Action<Container> SelectedContainerFunc;
        public Container[] exclusions;

        public Vector3 position;
        public Quaternion rotation;

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

            position = container.Position + (Vector3.back * 30) + (Vector3.up * 10);
            rotation = Quaternion.identity;

            if (!InputUtil.GetSelectContainer() || _doneSelecting)
                return;
            
            _doneSelecting = true;
            SelectedContainerFunc?.Invoke(container);
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
                _selectableContainers = FindObjectsOfType<Container>().Where((container) => exclusions.All(excluded => true/*excluded.id != container.id*/)).OrderBy((container) => container.Position).ToArray();
                _selectTimer.Start();
            }
        }
    }
}