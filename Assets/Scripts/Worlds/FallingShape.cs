using System.Linq;
using Sabotris.IO;
using Sabotris.Network.Packets.Block;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Worlds
{
    public class FallingShape : Shape
    {
        public new Vector3 RawPosition;
        public bool removed;

        private Vector3Int _startPosition;
        private Vector3Int _targetPosition;
        private float _velocity;

        private void Start()
        {
            foreach (var (blockId, blockPos) in Offsets)
                CreateBlock(blockId, blockPos);
            
            transform.localScale = Vector3.zero;

            _startPosition = RawPosition.Round(1);
            _targetPosition = parentContainer.GetDropToPosition(_startPosition);
        }

        private void FixedUpdate()
        {
            _targetPosition = Offsets.Select((offset) => parentContainer.GetDropToPosition(_startPosition + offset.Item2)).OrderBy((position) => position.y).Last();

            if (!removed)
            {
                _velocity += 0.01f.Delta();
                RawPosition += Vector3.down * _velocity;
            }

            if (RawPosition.y <= _targetPosition.y)
            {
                RawPosition = new Vector3(RawPosition.x, _targetPosition.y, RawPosition.z);
                _velocity = 0;
                if (!removed)
                {
                    removed = true;
                    if (gameController.ControllingContainer == parentContainer || (parentContainer is BotContainer && networkController.Server?.Running == true))
                    {
                        var blocks = Offsets.Select((offset) => (offset.Item1, offset.Item2 + RawPosition.Round(1), shapeColor)).ToArray();
                        var packet = new PacketBlockBulkCreate
                        {
                            ContainerId = parentContainer.Id,
                            Blocks = blocks
                        };
                        foreach (var (id, pos, color) in blocks)
                            parentContainer.CreateBlock(id, pos, color);
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