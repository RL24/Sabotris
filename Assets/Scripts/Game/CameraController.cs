using System.Numerics;
using Sabotris.IO;
using Sabotris.UI.Menu;
using Sabotris.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Sabotris
{
    public class CameraController : MonoBehaviour
    {
        public GameController gameController;
        public MenuController menuController;

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
            if (!camera || !gameController.ControllingContainer)
                return;

            var container = gameController.ControllingContainer;

            if (_targetContainer != container)
            {
                _targetShapePosition = container.transform.position + container.DropPosition;
                _targetContainer = container;
            }

            if (container.controllingShape)
                _targetShapePosition = container.controllingShape.transform.position;

            var shapePosition = _targetShapePosition;
            var containerPosition = container.transform.position;
            var targetPosition = (containerPosition + shapePosition) * 0.5f;

            var distance = (containerPosition - shapePosition).magnitude;
            var cameraDistance = distance / 2.0f / _aspectRatio / _tanFov;

            _rotationInput = menuController.IsInMenu || InputUtil.ShouldRotateShape()
                ? Vector3.zero
                : new Vector3(
                    InputUtil.GetCameraRotateYaw(),
                    InputUtil.GetCameraRotatePitch(),
                    InputUtil.GetCameraZoom()
                );

            Yaw += _rotationInput.x;
            Pitch -= _rotationInput.y;
            Zoom -= _rotationInput.z;

            Pitch = Mathf.Clamp(Pitch, -80, 80);
            Zoom = Mathf.Clamp(Zoom, 10, 20);

            cameraRotation = Quaternion.Euler(Pitch, Yaw, 0);
            if (IsSpectating)
            {
                var advance = -InputUtil.GetMoveAdvance() * Acceleration;
                var strafe = InputUtil.GetMoveStrafe() * Acceleration;
                var ascend = InputUtil.GetMoveAscend() * Acceleration;

                _velocity += transform.forward.Horizontal(true) * advance + transform.right * strafe + Vector3.up * ascend;
                cameraPosition += _velocity;
                _velocity *= Friction;
            }
            else
            {
                _velocity = Vector3.zero;
                cameraPosition = cameraRotation * new Vector3(0f, 0f, -cameraDistance - Zoom) + targetPosition;
            }
        }

        private void FixedUpdate()
        {
            var cameraTransform = camera.transform;
            var cameraTransformPosition = cameraTransform.position;
            var cameraTransformRotation = cameraTransform.rotation;

            var toPosition = cameraPosition;
            var toRotation = cameraRotation;
            var animationTime = GameSettings.Settings.gameCameraSpeed;

            if (menuController.IsInMenu)
            {
                var targetMenu = menuController.GetTargetMenu();
                toPosition = targetMenu.GetCameraPosition();
                toRotation = targetMenu.GetCameraRotation();
                animationTime = GameSettings.Settings.menuCameraSpeed;
            }

            if (IsSpectating)
                animationTime *= 0.5f;
            
            cameraTransform.position = Vector3.Lerp(cameraTransformPosition, toPosition, animationTime);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransformRotation, toRotation, animationTime);
        }

        public void ResetCamera()
        {
            var ct = camera.transform;
            ct.position = cameraPosition = _defaultPosition;
            ct.rotation = cameraRotation = _defaultRotation;
        }

        private bool IsSpectating => gameController.ControllingContainer is {dead: true} && !gameController.menuController.IsInMenu;
    }
}