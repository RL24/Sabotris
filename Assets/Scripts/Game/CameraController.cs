using Menu;
using Sabotris.Util;
using UnityEngine;

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

        private Vector3 _cameraPosition = Vector3.zero;
        private Quaternion _cameraRotation = Quaternion.identity;
        private Vector3 _rotationInput = Vector3.zero; // yaw pitch zoom

        private Container _targetContainer;
        private Vector3 _targetShapePosition;

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Zoom { get; set; }

        private void Start()
        {
            var ct = camera.transform;
            _cameraPosition = _defaultPosition = ct.position;
            _cameraRotation = _defaultRotation = ct.rotation;

            _aspectRatio = Screen.width / (float) Screen.height;
            _tanFov = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2.0f);
        }

        private void Update()
        {
            if (!camera || gameController.ControllingContainer == null)
                return;

            var container = gameController.ControllingContainer;

            if (_targetContainer != container)
            {
                _targetShapePosition = container.transform.position + container.DropPosition;
                _targetContainer = container;
            }

            if (container.controllingShape != null)
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

            _cameraRotation = Quaternion.Euler(Pitch, Yaw, Roll);
            _cameraPosition = _cameraRotation * new Vector3(0f, 0f, -cameraDistance - Zoom) + targetPosition;
        }

        private void FixedUpdate()
        {
            var cameraTransform = camera.transform;
            var cameraTransformPosition = cameraTransform.position;
            var cameraTransformRotation = cameraTransform.rotation;

            var toPosition = _cameraPosition;
            var toRotation = _cameraRotation;
            var animationTime = GameSettings.GameTransitionSpeed;

            if (menuController.IsInMenu)
            {
                var targetMenu = menuController.GetTargetMenu();
                toPosition = targetMenu.GetCameraPosition();
                toRotation = targetMenu.GetCameraRotation();
                animationTime = GameSettings.MenuCameraSpeed;
            }
            
            cameraTransform.position = Vector3.Lerp(cameraTransformPosition, toPosition, animationTime);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransformRotation, toRotation, animationTime);
        }

        public void ResetCamera()
        {
            var ct = camera.transform;
            ct.position = _cameraPosition = _defaultPosition;
            ct.rotation = _cameraRotation = _defaultRotation;
        }
    }
}