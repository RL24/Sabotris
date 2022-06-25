using System;
using System.Linq;
using Sabotris.Game;
using Sabotris.Util;
using Sabotris.Util.Input;
using UnityEngine;

namespace Sabotris.Worlds.Tutorial
{
    public class TutorialHelper : MonoBehaviour
    {
        public InputController inputController;
        public CameraController cameraController;
        public Textures textures;
        public TutorialItem yawItem, pitchItem, rollItem;

        public Shape shape;

        private bool _isGamepad;

        private void Update()
        {
            if (inputController.AnyKeyPressed())
                _isGamepad = false;
            if (inputController.AnyGamepadButtonPressed())
                _isGamepad = true;

            if (!shape)
                return;

            transform.position = shape.transform.position;
            var cameraRotation = cameraController.cameraRotation;
            var rotationToCamera = Quaternion.LookRotation(transform.position - cameraController.cameraPosition, Vector3.up);
            
            var snapRotation = Quaternion.Euler((cameraRotation.eulerAngles + Vector3.up * 45).Round(90) * Vector3Int.up);

            var yawRotation = cameraRotation.Yaw();
            var yaw = snapRotation.eulerAngles.y;
            var pitch = rotationToCamera.Pitch();
            var yawDif = Quaternion.Angle(yawRotation, snapRotation);

            var offsets = shape.Offsets.Select((x) => x.Item2).ToArray();
            var (min, max) = offsets.MinMax();
            min -= Vector3Int.one;
            max += Vector3Int.one;

            var (pitchX, pitchZ) = (yaw).AngleToVec();
            var (rollX, rollZ) = (yaw + 90).AngleToVec();

            if (Math.Abs(pitchX) > 0.1)
                pitchX = pitchX > 0 ? max.x : min.x;
            if (Math.Abs(pitchZ) > 0.1)
                pitchZ = pitchZ > 0 ? max.z : min.z;

            if (Math.Abs(rollX) > 0.1)
                rollX = rollX > 0 ? max.x : min.x;
            if (Math.Abs(rollZ) > 0.1)
                rollZ = rollZ > 0 ? max.z : min.z;
            
            var up = pitch > 0;
            yawItem.lerpPosition = new Vector3(0, up ? max.y : min.y, 0);
            pitchItem.lerpPosition = new Vector3(pitchX, 0, pitchZ);
            rollItem.lerpPosition = new Vector3(rollX, 0, rollZ);
            
            yawItem.lerpRotation = rotationToCamera;
            pitchItem.lerpRotation = Quaternion.Euler(snapRotation.eulerAngles.x, snapRotation.eulerAngles.y + 90, snapRotation.eulerAngles.z);
            rollItem.lerpRotation = snapRotation;
            
            pitchItem.itemRotation = rotationToCamera;
            rollItem.itemRotation = rotationToCamera;

            yawItem.left.sprite = textures.GetMapped(inputController.GetPath(inputController.rotateYawLeft, _isGamepad));
            yawItem.right.sprite = textures.GetMapped(inputController.GetPath(inputController.rotateYawRight, _isGamepad));

            if (yawDif < 45)
            {
                pitchItem.left.sprite = textures.GetMapped(inputController.GetPath(inputController.rotatePitchUp, _isGamepad));
                pitchItem.right.sprite = textures.GetMapped(inputController.GetPath(inputController.rotatePitchDown, _isGamepad));
                
                rollItem.left.sprite = textures.GetMapped(inputController.GetPath(inputController.rotateRollLeft, _isGamepad));
                rollItem.right.sprite = textures.GetMapped(inputController.GetPath(inputController.rotateRollRight, _isGamepad));
            }
            else
            {
                pitchItem.left.sprite = textures.GetMapped(inputController.GetPath(inputController.rotateRollRight, _isGamepad));
                pitchItem.right.sprite = textures.GetMapped(inputController.GetPath(inputController.rotateRollLeft, _isGamepad));

                rollItem.left.sprite = textures.GetMapped(inputController.GetPath(inputController.rotatePitchUp, _isGamepad));
                rollItem.right.sprite = textures.GetMapped(inputController.GetPath(inputController.rotatePitchDown, _isGamepad));
            }
        }
    }
}