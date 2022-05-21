using System;
using Sabotris.Game;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Worlds
{
    public class FallingBlock : MonoBehaviour
    {
        public GameController gameController;
        public NetworkController networkController;
        public Container parentContainer;
        
        public Guid Id;
        public Color? BlockColor;
        public Vector3 RawPosition;
        public bool removed;

        private Vector3Int _startPosition;
        private Vector3Int _targetPosition;
        private float _velocity;

        private void Start()
        {
            if (BlockColor != null)
                foreach (var ren in GetComponentsInChildren<Renderer>())
                    ren.material.color = BlockColor ?? Color.white;

            transform.localScale = Vector3.zero;

            _startPosition = RawPosition.Round(1);
            _targetPosition = parentContainer.GetDropToPosition(_startPosition);
        }

        private void FixedUpdate()
        {
            _targetPosition = parentContainer.GetDropToPosition(_startPosition);

            if (!removed)
            {
                _velocity += 0.01f.Delta();
                RawPosition += Vector3.down * _velocity;
            }

            if (RawPosition.y <= _targetPosition.y)
            {
                RawPosition = _targetPosition;
                _velocity = 0;
                if (!removed)
                {
                    removed = true;
                    if (gameController.ControllingContainer == parentContainer || (parentContainer is BotContainer && networkController.Server?.Running == true))
                    {
                        var packet = new PacketBlockCreate
                        {
                            Id = Guid.NewGuid(),
                            ContainerId = parentContainer.Id,
                            Position = _targetPosition,
                            Color = BlockColor ?? Color.white
                        };
                        parentContainer.CreateBlock(packet.Id, packet.Position, packet.Color);
                        networkController.Client?.SendPacket(packet);
                    }

                    Destroy(gameObject);
                }
            }
            
            transform.position = Vector3.Lerp(transform.position, parentContainer.transform.position + RawPosition, GameSettings.Settings.gameTransitionSpeed.Delta());
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, GameSettings.Settings.gameTransitionSpeed.FixedDelta());
        }
    }
}