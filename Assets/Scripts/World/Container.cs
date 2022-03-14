using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.UI.Menu;
using Sabotris.Util;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sabotris
{
    public class Container : MonoBehaviour
    {
        public const int Radius = 2;
        private static readonly Vector3Int BottomLeft = new Vector3Int(-Radius, 1, -Radius);
        private static readonly Vector3Int TopRight = new Vector3Int(Radius, 25, Radius);

        private static readonly Vector3Int GenerateBottomLeft = new Vector3Int(-Radius, 0, -Radius);
        private static readonly Vector3Int GenerateTopRight = new Vector3Int(Radius, 24, Radius);

        private const float ClearLayerSpeed = 0.1f;
        private const float ClearedLayerDropSpeed = 0.25f;
        private const float DropNewShapeSpeed = 0.4f;

        public Shape shapeTemplate;
        public Block blockTemplate;

        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public AudioController audioController;
        public TMP_Text nameText, dropSpeedText;

        public ulong id;
        [SerializeField] private string containerName;
        private PlayerScore _score = new PlayerScore(0, 0);
        public bool dead;

        public Shape controllingShape;

        private readonly Dictionary<Guid, Shape> _shapes = new Dictionary<Guid, Shape>();
        private readonly Dictionary<Guid, Block> _blocks = new Dictionary<Guid, Block>();

        public Vector3Int DropPosition { get; protected set; } = new Vector3Int(0, 20, 0);

        public int DropSpeedMs { get; private set; } = 1000;
        public const int DropSpeedFastMs = 10;

        public int MoveSpeedMs { get; } = 200;
        public int MoveResetSpeedMs { get; } = 50;

        public int RotateThreshold { get; } = 90;

        protected virtual void Start()
        {
            networkController.Client.RegisterListener(this);
        }

        private void OnDestroy()
        {
            networkController.Client.DeregisterListener(this);
        }

        private void Update()
        {
            if (dropSpeedText)
                dropSpeedText.text = $"Drop Speed: {(DropSpeedMs == 0 ? 100 : 1000f / DropSpeedMs):F1}";
        }

        public void StartDropping((Guid, Vector3Int)[] offsets = null)
        {
            if ((gameController.ControllingContainer != this && !IsDemo()) || dead)
                return;

            offsets ??= ShapeUtil.Generate(networkController.Client.LobbyData.BlocksPerShape, networkController.Client.LobbyData.GenerateVerticalBlocks, GenerateBottomLeft, GenerateTopRight);

            if (!DoesCollide(offsets.Select((offset) => offset.Item2 + DropPosition).ToArray()))
            {
                var shape = CreateShape(Guid.NewGuid(), DropPosition, offsets, Random.ColorHSV(0, 1, 0.7f, 0.7f, 1, 1));
                shape.StartDropping();
                controllingShape = shape;

                if (!IsDemo())
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
                foreach (var block in blocks)
                    RemoveBlock(block, index++, blocks.Length);

                dead = true;

                if (!IsDemo())
                    networkController.Client.SendPacket(new PacketPlayerDead
                    {
                        Id = id
                    });
            }
        }

        public void LockShape(Shape shape, Vector3Int[] addedBlocks)
        {
            controllingShape = null;

            foreach (var block in shape.Blocks)
            {
                _blocks.Add(block.Key, block.Value);
                block.Value.RawPosition = shape.RawPosition + shape.Offsets.First((pair) => pair.Item1 == block.Key).Item2;
            }

            if (gameController.ControllingContainer != this && !IsDemo())
                return;

            if (!IsDemo())
            {
                audioController.shapeLock.volume = 1f * (GameSettings.Settings.MasterVolume * 0.01f);
                audioController.shapeLock.Play();
                
                networkController.Client.SendPacket(new PacketShapeLock
                {
                    Id = shape.id,
                    Offsets = addedBlocks
                });
            }

            StartCoroutine(StartClearingLayers(addedBlocks));
        }

        private IEnumerator StartClearingLayers(Vector3Int[] addedBlocks)
        {
            yield return new WaitForSeconds(ClearLayerSpeed);

            var clearedLayers = ClearLayers(addedBlocks);

            if (clearedLayers.Count > 0)
            {
                yield return new WaitForSeconds(ClearedLayerDropSpeed);

                clearedLayers.Sort((a, b) => b.CompareTo(a));
                foreach (var layer in clearedLayers)
                    foreach (var block in _blocks.Values.Where((block) => block.RawPosition.y > layer))
                    {
                        block.RawPosition += Vector3Int.down;
                        block.shifted = true;
                    }

                if (!IsDemo())
                {
                    audioController.layerDelete.volume = 1f * (GameSettings.Settings.MasterVolume * 0.01f);
                    audioController.layerDelete.Play();
                    
                    networkController.Client.SendPacket(new PacketLayerMove
                    {
                        ContainerId = id,
                        Layers = clearedLayers.ToArray()
                    });
                }
            }

            yield return new WaitForSeconds(DropNewShapeSpeed);

            StartDropping(IsDemo() ? (this as DemoContainer)?.GetNextOffsets() : null);
        }

        private Shape CreateShape(Guid shapeId, Vector3Int position, (Guid, Vector3Int)[] offsets, Color? color = null)
        {
            var shape = Instantiate(shapeTemplate, position, Quaternion.identity);
            shape.name = $"Shape-{shapeId}";

            shape.id = shapeId;
            shape.Offsets = offsets;
            shape.color = color;

            shape.gameController = gameController;
            shape.menuController = menuController;
            shape.networkController = networkController;
            shape.cameraController = cameraController;
            shape.audioController = audioController;
            shape.parentContainer = this;

            shape.transform.SetParent(transform, false);

            _shapes.Add(shapeId, shape);

            return shape;
        }

        public void RemoveShape(Guid shapeId)
        {
            if (!_shapes.TryGetValue(shapeId, out var shape))
                return;

            Destroy(shape.gameObject);
            _shapes.Remove(shapeId);
        }

        private void CreateBlock(Vector3Int position, Color color)
        {
            var blockId = Guid.NewGuid();
            var block = Instantiate(blockTemplate, position, Quaternion.identity);
            block.name = $"Block-{blockId}";
            block.RawPosition = position;
            block.color = color;

            block.id = blockId;

            block.transform.SetParent(transform, false);

            _blocks.Add(blockId, block);
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

        private List<int> ClearLayers(IEnumerable<Vector3Int> addedBlocks)
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

            if (!IsDemo())
            {
                if (deletedBlocks.Any())
                    networkController.Client.SendPacket(new PacketBlockBulkRemove
                    {
                        ContainerId = id,
                        Ids = deletedBlocks.ToArray()
                    });

                if (clearingLayers.Any())
                    networkController.Client.SendPacket(new PacketPlayerScore
                    {
                        Id = id,
                        Score = new PlayerScore(_score.Score + (deletedBlocks.Count * clearingLayers.Count), _score.ClearedLayers)
                    });
            }

            return clearingLayers;
        }

        private IEnumerator CreateEndBlocksForScore()
        {
            var score = _score.Score;

            var min = new Vector3Int(-Radius, 1, -Radius);
            var max = new Vector3Int(Radius, Mathf.CeilToInt(_score.Score / (float) Math.Pow(Radius * 2f + 1, 2)), Radius);

            for (var z = max.z; z >= min.z && score > 0; z--)
            for (var y = min.y; y <= max.y && score > 0; y++)
            for (var x = min.x; x <= max.x && score > 0; x++)
            {
                var pos = new Vector3Int(x * (Mathf.Repeat(y, 2) == 0 ? -1 : 1) * (Mathf.Repeat(z, 2) == 0 ? 1 : -1), y, z);
                var color = Random.ColorHSV(0, 1, 0.7f, 0.7f, 1, 1);

                CreateBlock(pos, color);
                score--;

                yield return new WaitForSeconds(0.075f);
            }
        }

        [PacketListener(PacketTypeId.GameEnd, PacketDirection.Client)]
        public void OnGameEnd(PacketGameEnd packet)
        {
            if (!IsDemo())
                StartCoroutine(CreateEndBlocksForScore());
        }

        [PacketListener(PacketTypeId.ShapeCreate, PacketDirection.Client)]
        public void OnShapeCreate(PacketShapeCreate packet)
        {
            if (packet.ContainerId != id)
                return;

            CreateShape(packet.Id, packet.Position, packet.Offsets, packet.Color);
        }

        [PacketListener(PacketTypeId.LayerMove, PacketDirection.Client)]
        public void OnLayerMove(PacketLayerMove packet)
        {
            if (packet.ContainerId != id)
                return;

            foreach (var layer in packet.Layers)
                foreach (var block in _blocks.Values.Where((block) => block.RawPosition.y > layer))
                {
                    block.RawPosition += Vector3Int.down;
                    block.shifted = true;
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

            var index = 0;
            var blocks = _blocks.Values.OrderBy((block) => block.RawPosition.magnitude).ToArray();
            foreach (var block in blocks)
                RemoveBlock(block, index++, blocks.Length);

            dead = true;
        }

        [PacketListener(PacketTypeId.PlayerScore, PacketDirection.Client)]
        public void OnPlayerScore(PacketPlayerScore packet)
        {
            DropSpeedMs = Mathf.Clamp(DropSpeedMs - 50, 100, 1000);

            if (packet.Id != id)
                return;

            _score = packet.Score;
        }

        public bool IsDemo() => this is DemoContainer;

        public string ContainerName
        {
            get => containerName;
            set
            {
                if (value == ContainerName)
                    return;

                containerName = value;

                nameText.text = value;
            }
        }
    }
}