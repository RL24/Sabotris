using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sabotris
{
    public class Container : MonoBehaviour, ISerializationCallbackReceiver
    {
        public const int Radius = 2;
        private static readonly Vector3Int BottomLeft = new Vector3Int(-Radius, 1, -Radius);
        private static readonly Vector3Int TopRight = new Vector3Int(Radius, 21, Radius);
        
        private static readonly Vector3Int GenerateBottomLeft = new Vector3Int(-Radius, 0, -Radius);
        private static readonly Vector3Int GenerateTopRight = new Vector3Int(Radius, 20, Radius);

        private const float ClearLayerSpeed = 0.5f;
        public const float ClearedLayerDropSpeed = 0.5f;
        private const float DropNewShapeSpeed = 0.5f;
        
        public Shape shapeTemplate;
        
        public GameController gameController;
        public NetworkController networkController;
        public CameraController cameraController;

        public long id;
        public string containerName;

        public Shape controllingShape;

        private readonly Dictionary<Guid, Shape> _shapes = new Dictionary<Guid, Shape>();
        private readonly Dictionary<Guid, Block> _blocks = new Dictionary<Guid, Block>();

        public Vector3Int DropPosition { get; set; } = new Vector3Int(0, 10, 0);

        public int DropSpeedMs { get; set; } = 1000;
        public int DropSpeedFastMs { get; set; } = 50;
        
        public int MoveSpeedMs { get; set; } = 200;
        public int MoveResetSpeedMs { get; set; } = 50;
        
        public int RotateThreshold { get; set; } = 90;

        private void Start()
        {
            networkController.Client.RegisterListener(this);
        }

        private void OnDestroy()
        {
            networkController.Client.DeregisterListener(this);
        }

        private void Update()
        {
        }

        public void StartDropping()
        {
            if (gameController.ControllingContainer != this)
                return;
            
            var offsets = ShapeUtil.Generate(4, false, GenerateBottomLeft, GenerateTopRight);

            if (!DoesCollide(offsets.Select((offset) => offset.Value + DropPosition).ToArray()))
            {
                var shape = CreateShape(Guid.NewGuid(), DropPosition, offsets, Random.ColorHSV(0, 1, 0.7f, 0.7f, 1, 1));
                shape.StartDropping();
                controllingShape = shape;
                
                networkController.Client.SendPacket(new PacketShapeCreate
                {
                    ContainerId = id,
                    Id = shape.id,
                    Position = DropPosition,
                    Offsets = shape.Offsets,
                    Color = shape.color ?? Color.white
                });
            }
            else
            {
                var index = 0;
                var blocks = _blocks.Values.OrderBy((block) => block.RawPosition.magnitude).ToArray();
                var indices = new List<Pair<Guid, int>>();
                foreach (var block in blocks)
                {
                    RemoveBlock(block, index++, blocks.Length);
                    indices.Add(new Pair<Guid, int>(block.id, index));
                }

                networkController.Client.SendPacket(new PacketPlayerDead
                {
                    Id = id,
                    BlockIndices = indices.ToArray()
                });
            }
        }
        
        public void LockShape(Shape shape, Vector3Int[] addedBlocks)
        {
            controllingShape = null;
            
            foreach (var block in shape.Blocks)
                _blocks.Add(block.Key, block.Value);

            if (gameController.ControllingContainer != this)
                return;
            
            networkController.Client.SendPacket(new PacketShapeLock
            {
                Id = shape.id,
                Offsets = addedBlocks
            });
            
            StartCoroutine(StartClearingLayers(addedBlocks));
        }

        private IEnumerator StartClearingLayers(Vector3Int[] addedBlocks)
        {
            yield return new WaitForSeconds(ClearLayerSpeed);

            var clearedLayers = ClearLayers(addedBlocks);

            if (clearedLayers.Count > 0)
            {
                yield return new WaitForSeconds(ClearedLayerDropSpeed);

                var movedBlocks = new List<Block>();
                clearedLayers.Sort((a, b) => b.CompareTo(a));
                foreach (var layer in clearedLayers)
                    foreach (var block in _blocks.Values.Where((block) => block.RawPosition.y > layer))
                    {
                        block.RawPosition += Vector3Int.down;
                        movedBlocks.Add(block);
                    }
            
                networkController.Client.SendPacket(new PacketBlockBulkMove
                {
                    ContainerId = id,
                    Positions = movedBlocks.Select((block) => new Pair<Guid, Vector3Int>(block.id, block.RawPosition)).ToArray()
                });
            }
            
            yield return new WaitForSeconds(DropNewShapeSpeed);
            
            StartDropping();
        }
        
        public Shape CreateShape(Guid id, Vector3Int position, Pair<Guid, Vector3Int>[] offsets, Color? color = null)
        {
            var shape = Instantiate(shapeTemplate, position, Quaternion.identity);
            shape.name = $"Shape_{id}";

            shape.id = id;
            shape.Offsets = offsets;
            shape.color = color;

            shape.gameController = gameController;
            shape.networkController = networkController;
            shape.cameraController = cameraController;
            shape.parentContainer = this;
            
            shape.transform.SetParent(transform, false);
            
            _shapes.Add(id, shape);
            
            return shape;
        }

        public void RemoveShape(Guid shapeId)
        {
            if (!_shapes.TryGetValue(shapeId, out var shape))
                return;
            
            Destroy(shape.gameObject);
            _shapes.Remove(shapeId);
        }

        private void RemoveBlock(Guid blockId, int index = -1, int max = -1)
        {
            if (!_blocks.TryGetValue(blockId, out var block))
                return;
            
            RemoveBlock(block, index, max);
        }

        private void RemoveBlock(Block block, int index = -1, int max = -1)
        {
            StartCoroutine(block.Remove(index, max));
            _blocks.Remove(block.id);
        }

        public bool DoesCollide(Vector3Int[] absolutePositions)
        {
            if (absolutePositions.Any((pos) => pos.IsOutside(BottomLeft, TopRight)))
                return true;
            foreach (var block in _blocks.Values)
                foreach (var pos in absolutePositions)
                    if (pos.Equals(block.RawPosition))
                        return true;
            return false;
        }

        private List<int> ClearLayers(Vector3Int[] addedBlocks)
        {
            var distinctLayers = addedBlocks.Select((vec) => vec.y).Distinct().ToArray();
            var clearingLayers = new List<int>();
            var deletedBlocks = new List<Guid>();
            foreach (var layer in distinctLayers)
            {
                var blocksInLayer = _blocks.Count((block) => block.Value.RawPosition.y == layer);
                var minBlockCountToClear = Math.Pow(Radius * 2 + 1, 2);
                if (blocksInLayer < minBlockCountToClear)
                    continue;
                
                var blocksToRemove = _blocks.Where((block) => block.Value.RawPosition.y == layer).Select((block) => block.Value).ToArray();
                foreach (var block in blocksToRemove)
                {
                    deletedBlocks.Add(block.id);
                    RemoveBlock(block);
                }

                clearingLayers.Add(layer);
            }
            
            networkController.Client.SendPacket(new PacketBlockBulkRemove
            {
                ContainerId = id,
                Ids = deletedBlocks.ToArray()
            });

            return clearingLayers;
        }

        [PacketListener(PacketTypeId.ShapeCreate, PacketDirection.Client)]
        public void OnShapeCreate(PacketShapeCreate packet)
        {
            if (packet.ContainerId != id)
                return;
            
            CreateShape(packet.Id, packet.Position, packet.Offsets, packet.Color);
        }

        [PacketListener(PacketTypeId.BlockBulkMove, PacketDirection.Client)]
        public void OnBlockBulkMove(PacketBlockBulkMove packet)
        {
            if (packet.ContainerId != id)
                return;
            
            foreach (var pair in packet.Positions)
            {
                if (!_blocks.TryGetValue(pair.Key, out var block))
                    continue;
                block.RawPosition = pair.Value;
            }
        }

        [PacketListener(PacketTypeId.BlockBulkRemove, PacketDirection.Client)]
        public void OnBlockBulkRemove(PacketBlockBulkRemove packet)
        {
            if (packet.ContainerId != id)
                return;
            
            foreach (var blockId in packet.Ids)
                RemoveBlock(blockId);
        }

        [PacketListener(PacketTypeId.PlayerDead, PacketDirection.Client)]
        public void OnPlayerDead(PacketPlayerDead packet)
        {
            if (packet.Id != id)
                return;

            foreach (var blockIndex in packet.BlockIndices)
                RemoveBlock(blockIndex.Key, blockIndex.Value, packet.BlockIndices.Length);
        }
        
        #region Serialize Fields

        public Shape[] serializedShapes;
        public Block[] serializedBlocks;
        
        public void OnBeforeSerialize()
        {
            serializedShapes = _shapes.Values.ToArray();
            serializedBlocks = _blocks.Values.ToArray();
        }

        public void OnAfterDeserialize()
        {
        }
        
        #endregion
    }
}
