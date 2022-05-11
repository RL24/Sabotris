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
using Translations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sabotris
{
    public enum Movement
    {
        X,
        Z,
        RotateX,
        RotateY,
        RotateZ
    }
    
    public class Container : MonoBehaviour
    {
        protected int Radius => (networkController ? networkController.Client?.LobbyData?.PlayFieldSize : null) ?? 2;
        private Vector3Int BottomLeft => new Vector3Int(-Radius, 1, -Radius);
        private Vector3Int TopRight => new Vector3Int(Radius, 25, Radius);

        private Vector3Int GenerateBottomLeft => new Vector3Int(-Radius, 0, -Radius);
        private Vector3Int GenerateTopRight => new Vector3Int(Radius, 24, Radius);

        private const float ClearLayerSpeed = 0.1f;
        private const float ClearedLayerDropSpeed = 0.25f;
        private const float DropNewShapeSpeed = 0.4f;

        public Shape shapeTemplate;
        public Block blockTemplate;

        public World world;
        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public AudioController audioController;

        public Vector3 rawPosition;
        public GameObject floor;
        public TMP_Text nameText, dropSpeedText;

        public Guid id;
        public ulong steamId;
        [SerializeField] private string containerName;
        private PlayerScore _score = new PlayerScore(0, 0);
        public bool dead;

        private Shape _controllingShape;

        private readonly Dictionary<Guid, Shape> _shapes = new Dictionary<Guid, Shape>();
        private readonly Dictionary<Guid, Block> _blocks = new Dictionary<Guid, Block>();

        public Vector3Int DropPosition { get; } = new Vector3Int(0, 20, 0);

        private int _dropSpeedMs = 1000;
        public const int DropSpeedFastMs = 10;
        private int DropSpeedIncrementMs => Math.Max(5, 40 / (world == null ? 1 : world.Containers.Count));

        public int MoveSpeedMs { get; } = 200;
        public int MoveResetSpeedMs { get; } = 50;

        public int RotateThreshold { get; } = 90;

        protected virtual void Start()
        {
            if (networkController)
                networkController.Client?.RegisterListener(this);

            if (floor)
                floor.transform.localScale = new Vector3(Radius * 2 + 1, 1, Radius * 2 + 1);
            if (nameText)
                nameText.transform.position = new Vector3(nameText.transform.position.x, -2, -(Radius * 2 + 1) * 0.5f);
            if (dropSpeedText)
                dropSpeedText.transform.position = new Vector3(dropSpeedText.transform.position.x, -4, -(Radius * 2 + 1) * 0.5f);
        }

        private void OnDestroy()
        {
            if (networkController)
                networkController.Client?.DeregisterListener(this);
        }

        protected virtual void Update()
        {
            if (dropSpeedText)
                dropSpeedText.text = Localization.Translate(TranslationKey.GameContainerDropSpeed, Math.Round((DropSpeedMs == 0 ? 100 : 1000f / DropSpeedMs) * 10) / 10);

            transform.position = Vector3.Lerp(transform.position, rawPosition, GameSettings.Settings.gameTransitionSpeed * 0.5f);
        }

        public void StartDropping((Guid, Vector3Int)[] offsets = null)
        {
            if (dead || this is BotContainer && networkController.Server?.Running == false)
                return;

            var blocksPerShape = (networkController ? networkController.Client?.LobbyData.BlocksPerShape : null) ?? 4;
            var generateVerticalBlocks = (networkController ? networkController.Client?.LobbyData.GenerateVerticalBlocks : null) ?? false;
            
            offsets ??= ShapeUtil.Generate(blocksPerShape, generateVerticalBlocks, GenerateBottomLeft, GenerateTopRight);

            if (!DoesCollide(offsets.Select((offset) => offset.Item2 + DropPosition).ToArray()))
            {
                var shape = CreateShape(Guid.NewGuid(), DropPosition, offsets, Random.ColorHSV(0, 1, 0.7f, 0.7f, 1, 1));
                shape.StartDropping();
                ControllingShape = shape;

                if (ShouldSendPacket())
                    networkController.Client?.SendPacket(new PacketShapeCreate
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

                if (ShouldSendPacket())
                    networkController.Client?.SendPacket(new PacketPlayerDead
                    {
                        Id = id
                    });
            }
        }

        public void LockShape(Shape shape, Vector3Int[] addedBlocks)
        {
            ControllingShape = null;

            foreach (var block in shape.Blocks)
            {
                _blocks.Add(block.Key, block.Value);
                block.Value.RawPosition = shape.RawPosition + shape.Offsets.First((pair) => pair.Item1 == block.Key).Item2;
            }

            if (gameController.ControllingContainer != this && !(this is BotContainer && networkController.Server?.Running == true || this is DemoContainer))
                return;

            if (audioController)
                audioController.shapeLock.PlayModifiedSound(AudioController.GetGameVolume());
            if (ShouldSendPacket())
                networkController.Client?.SendPacket(new PacketShapeLock
                {
                    Id = shape.id,
                    LockPos = shape.RawPosition,
                    LockRot = shape.RawRotation,
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

                clearedLayers.Sort((a, b) => b.CompareTo(a));
                foreach (var layer in clearedLayers)
                    foreach (var block in _blocks.Values.Where((block) => block.RawPosition.y > layer))
                    {
                        block.RawPosition += Vector3Int.down;
                        block.shifted = true;
                    }
                
                if (audioController)
                    audioController.layerDelete.PlayModifiedSound(AudioController.GetGameVolume());
                if (networkController)
                    networkController.Client?.SendPacket(new PacketLayerMove
                    {
                        ContainerId = id,
                        Layers = clearedLayers.ToArray()
                    });
            }

            yield return new WaitForSeconds(DropNewShapeSpeed);

            StartDropping();
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

            if (ShouldSendPacket())
            {
                if (deletedBlocks.Any())
                    networkController.Client?.SendPacket(new PacketBlockBulkRemove
                    {
                        ContainerId = id,
                        Ids = deletedBlocks.ToArray()
                    });

                if (clearingLayers.Any())
                    networkController.Client?.SendPacket(new PacketPlayerScore
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
            if (!(this is DemoContainer))
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
            DropSpeedMs = Mathf.Clamp(DropSpeedMs - DropSpeedIncrementMs, 100, 1000);

            if (packet.Id != id)
                return;

            _score = packet.Score;
        }

        public virtual (float, float) GetMovement()
        {
            return gameController.ControllingContainer == this ? (InputUtil.GetMoveAdvance(), InputUtil.GetMoveStrafe()) : (0, 0);
        }

        public virtual bool DoFastDrop()
        {
            return gameController.ControllingContainer == this && InputUtil.GetMoveDown() && !menuController.IsInMenu;
        }

        public virtual bool IsControllingShape(Shape shape, bool notInMenu = false)
        {
            return ControllingShape == shape && (!notInMenu || !menuController.IsInMenu);
        }

        public virtual bool ShouldRotateShape()
        {
            return InputUtil.ShouldRotateShape();
        }

        public virtual bool ShouldSendPacket()
        {
            return networkController;
        }

        protected virtual void OnControllingShapeCreated(Shape shape)
        {
        }

        protected virtual int GetDropSpeed()
        {
            return _dropSpeedMs;
        }

        public Shape ControllingShape
        {
            get => _controllingShape;
            set
            {
                if (_controllingShape == value)
                    return;
                
                _controllingShape = value;
                
                OnControllingShapeCreated(value);
            }
        }

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
        
        public int DropSpeedMs
        {
            get => GetDropSpeed();
            private set
            {
                if (_dropSpeedMs == value)
                    return;
                
                _dropSpeedMs = value;

                if (audioController)
                    audioController.music.pitch = Mathf.Min(audioController.music.pitch + 0.01f, 2);
            }
        }
    }
}