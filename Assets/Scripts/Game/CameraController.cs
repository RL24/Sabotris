using System;
using System.Collections;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets.Spectator;
using Sabotris.Powers;
using Sabotris.UI.Menu;
using Sabotris.UI.Menu.Menus;
using Sabotris.Util;
using Sabotris.Util.Input;
using Sabotris.Worlds;
using UnityEngine;

namespace Sabotris.Game
{
    public class CameraController : MonoBehaviour
    {
        private const int CameraAngleSnap = 18;
        
        public InputController inputController;
        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public ContainerSelectorController containerSelector;
        public World world;
        public SpectatorController spectatorControllerPrefab;

        public new Camera camera;
        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;

        private float _aspectRatio;
        private float _tanFov;

        public Vector3 cameraPosition = Vector3.zero;
        public Quaternion cameraRotation = Quaternion.identity;
        private Vector3 _rotationInput = Vector3.zero; // yaw pitch zoom

        private const float Acceleration = 0.002f;
        private const float Friction = 0.98f;
        private Vector3 _velocity = Vector3.zero;

        private Container _targetContainer;
        private Vector3 _targetShapePosition;

        public float Yaw { get; private set; }
        private float Pitch { get; set; }
        private float Zoom { get; set; } = 15;

        private bool _isSpectating;
        public SpectatorController spectatorObject;

        private void Start()
        {
            var ct = camera.transform;
            cameraPosition = _defaultPosition = ct.position;
            cameraRotation = _defaultRotation = ct.rotation;

            _aspectRatio = Screen.width / (float) Screen.height;
            _tanFov = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2.0f);
        }

        private void Update()
        {
            IsSpectating = gameController.ControllingContainer is {dead: true} && !gameController.menuController.IsInMenu;
            
            if (!camera || !gameController.ControllingContainer)
                return;

            _rotationInput = menuController.IsInMenu || containerSelector.Active || inputController.ShouldRotateShape()
                ? Vector3.zero
                : new Vector3(
                    inputController.GetCameraRotateYaw(),
                    inputController.GetCameraRotatePitch(),
                    inputController.GetCameraZoom()
                );

            Yaw += _rotationInput.x;
            Pitch -= _rotationInput.y;
            Zoom -= _rotationInput.z;

            Pitch = Mathf.Clamp(Pitch, -80, 80);
            Zoom = Mathf.Clamp(Zoom, 5, 30);

            cameraRotation = Quaternion.Euler(Pitch, Yaw, 0);
            if (inputController.GetSnapCamera())
            {
                cameraRotation = Quaternion.Euler(cameraRotation.eulerAngles.Round(CameraAngleSnap));

                if (_rotationInput == Vector3.zero)
                {
                    Yaw = cameraRotation.eulerAngles.y;
                    Pitch = cameraRotation.Pitch();
                }
            }

            if (IsSpectating)
            {
                if (spectatorObject)
                {
                    const float upscale = 4f;
                    var targetPosition = spectatorObject.transform.position + Vector3.up * (Zoom - 8) / upscale;

                    var distance = (spectatorObject.transform.position - targetPosition).magnitude;
                    var cameraDistance = distance * upscale / _aspectRatio / _tanFov;
                
                    _velocity = Vector3.zero;
                    cameraPosition = cameraRotation * new Vector3(0f, 0f, -cameraDistance) + targetPosition;
                }
                else
                {
                    var advance = -inputController.GetMoveAdvance() * Acceleration;
                    var strafe = inputController.GetMoveStrafe() * Acceleration;
                    var ascend = inputController.GetMoveAscend() * Acceleration;

                    _velocity += transform.forward.Horizontal(true) * advance + transform.right * strafe + Vector3.up * ascend;
                    _velocity *= Friction;
                }
            }
            else
            {
                var container = gameController.ControllingContainer;

                if (_targetContainer != container)
                {
                    _targetShapePosition = container.transform.position + container.DropPosition;
                    _targetContainer = container;
                }

                if (container.ControllingShape)
                    _targetShapePosition = container.ControllingShape.transform.position + Vector3.up * ((30 - Zoom) * 0.25f);

                var containerPosition = container.transform.position;
                var targetPosition = (containerPosition + _targetShapePosition) * 0.5f;

                var distance = (containerPosition - _targetShapePosition).magnitude;
                var cameraDistance = distance / 2.0f / _aspectRatio / _tanFov;
                
                _velocity = Vector3.zero;
                cameraPosition = cameraRotation * new Vector3(0f, 0f, -cameraDistance - Zoom) + targetPosition;
            }
        }

        private void FixedUpdate()
        {
            var cameraTransform = camera.transform;
            var cameraTransformPosition = cameraTransform.position;
            var cameraTransformRotation = cameraTransform.rotation;
            
            cameraPosition += _velocity;

            var toPosition = cameraPosition;
            var toRotation = cameraRotation;
            var animationTime = GameSettings.Settings.gameCameraSpeed.FixedDelta();

            if (containerSelector.Active)
            {
                toPosition = containerSelector.position;
                toRotation = containerSelector.rotation;
                animationTime = GameSettings.Settings.menuCameraSpeed.FixedDelta();
            }
            
            if (menuController.IsInMenu)
            {
                var targetMenu = menuController.GetTargetMenu();
                toPosition = targetMenu.GetCameraPosition();
                toRotation = targetMenu.GetCameraRotation();
                animationTime = GameSettings.Settings.menuCameraSpeed.FixedDelta();
            }

            if (IsSpectating)
                animationTime *= 0.5f;

            cameraTransform.position = Vector3.Lerp(cameraTransformPosition, toPosition, animationTime);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransformRotation, toRotation, animationTime);
        }

        public void SetSelectingContainer(PowerUp powerUp, Func<Container, Container, IEnumerator> selectedContainerFunc, Container activatingContainer, Container[] exclusions = null)
        {
            containerSelector.PowerUp = powerUp;
            containerSelector.activatingContainer = activatingContainer;
            containerSelector.exclusions = exclusions;
            containerSelector.SelectedContainerFunc = (selectedContainer) =>
            {
                StartCoroutine(OnSelectedContainer(selectedContainerFunc, activatingContainer, selectedContainer));
            };
            containerSelector.PowerUpTimer = activatingContainer.PowerUpTimer;
            containerSelector.Active = true;
        }
        
        private IEnumerator OnSelectedContainer(Func<Container, Container, IEnumerator> selectedContainerFunc, Container activatingContainer, Container selectedContainer)
        {
            activatingContainer.SelectedContainer();
            yield return selectedContainerFunc?.Invoke(activatingContainer, selectedContainer);
            containerSelector.End();
            activatingContainer.StartDropping();
        }

        public void ResetCamera()
        {
            var ct = camera.transform;
            ct.position = cameraPosition = _defaultPosition;
            ct.rotation = cameraRotation = _defaultRotation;
        }

        private void CreateSpectator()
        {
            if (spectatorObject)
            {
                world.Spectators.Remove(Client.UserId);
                Destroy(spectatorObject.gameObject);
            }

            spectatorObject = Instantiate(spectatorControllerPrefab, new Vector3(cameraPosition.x, 1, cameraPosition.z), cameraRotation);

            spectatorObject.cameraController = this;
            spectatorObject.networkController = networkController;
            
            spectatorObject.transform.SetParent(transform.parent, true);
            
            world.Spectators.Add(Client.UserId, spectatorObject);
            
            networkController.Client?.SendPacket(new PacketSpectatorCreate
            {
                Id = Client.UserId,
                Position = spectatorObject.transform.position,
                Rotation = spectatorObject.transform.rotation
            });
        }

        private void DestroySpectator()
        {
            if (!spectatorObject)
                return;
            world.Spectators.Remove(Client.UserId);
            Destroy(spectatorObject.gameObject);
            spectatorObject = null;
            
            networkController.Client?.SendPacket(new PacketSpectatorRemove
            {
                Id = Client.UserId
            });
        }

        public bool IsSpectating
        {
            get => _isSpectating;
            set
            {
                if (_isSpectating == value)
                    return;
                
                _isSpectating = value;

                if (IsSpectating)
                    CreateSpectator();
                else
                    DestroySpectator();
            }
        }
    }
}