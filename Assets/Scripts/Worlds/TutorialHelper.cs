using System;
using System.Linq;
using Sabotris.Game;
using Sabotris.Util;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Sabotris.Worlds
{
    public class TutorialHelper : MonoBehaviour
    {
        public Textures textures;
        public CameraController cameraController;
        public TutorialItem yawItem, pitchItem, rollItem;

        public Shape shape;

        private void Update()
        {
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

            if (yawDif < 45)
            {
                pitchItem.left.sprite = textures.triangle;
                pitchItem.right.sprite = textures.cross;
                
                rollItem.left.sprite = textures.square;
                rollItem.right.sprite = textures.circle;
            }
            else
            {
                pitchItem.left.sprite = textures.circle;
                pitchItem.right.sprite = textures.square;

                rollItem.left.sprite = textures.triangle;
                rollItem.right.sprite = textures.cross;
            }
        }
    }
}