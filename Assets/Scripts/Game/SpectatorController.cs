using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets.Spectator;
using Sabotris.UI.Menu.Menus;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Game
{
    public class SpectatorController : MonoBehaviour
    {
        private const float Acceleration = 65f;
        private const float JumpHeight = 250f;
        private const float Drag = 0.6f;
        
        public CameraController cameraController;
        public NetworkController networkController;
        public Rigidbody rigidBody;

        private float _advance, _strafe;
        private bool _jump, _grounded;

        private Quaternion _rotation;

        private void Start()
        {
            _rotation = transform.rotation;
        }
        
        private void Update()
        {
            _advance = -InputUtil.GetMoveAdvance() * Acceleration;
            _strafe = InputUtil.GetMoveStrafe() * Acceleration;
            _jump = (_jump || InputUtil.GetMoveJump()) && _grounded;
        }

        private void FixedUpdate()
        {
            _grounded = Physics.Raycast(transform.position, Vector3.down, 0.3f);
            
            var velocity = cameraController.transform.forward.Horizontal(true) * _advance + cameraController.transform.right * _strafe;
            if (_jump)
            {
                velocity = new Vector3(velocity.x, JumpHeight, velocity.z);
                _jump = false;
            }

            rigidBody.velocity = new Vector3(rigidBody.velocity.x * Drag, rigidBody.velocity.y, rigidBody.velocity.z * Drag);
            
            rigidBody.AddForce(velocity, ForceMode.Acceleration);
            
            Physics.SyncTransforms();
            var prevRotation = transform.rotation;
            transform.LookAt(transform.position + velocity.Horizontal(true));
            Physics.SyncTransforms();
            _rotation = transform.rotation;
            transform.rotation = prevRotation;
            Physics.SyncTransforms();

            transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, GameSettings.Settings.gameTransitionSpeed.FixedDelta());
            
            networkController.Client?.SendPacket(new PacketSpectatorMove
            {
                Id = Client.UserId,
                Position = transform.position,
                Rotation = transform.rotation
            });
        }
    }
}