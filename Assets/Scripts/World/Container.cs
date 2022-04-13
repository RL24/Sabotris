using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sabotris.Audio;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Powers;
using Sabotris.UI.Menu;
using Sabotris.Util;
using TMPro;
using Sabotris.Translations;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Sabotris
{
    public class Container : MonoBehaviour
    {
        private int Radius => IsDemo() ? 2 : networkController.Client?.LobbyData?.PlayFieldSize ?? 2;
        private Vector3Int BottomLeft => new Vector3Int(-Radius, 1, -Radius);
        private Vector3Int TopRight => new Vector3Int(Radius, 25, Radius);

        private Vector3Int GenerateBottomLeft => new Vector3Int(-Radius, 0, -Radius);
        private Vector3Int GenerateTopRight => new Vector3Int(Radius, 24, Radius);

        private const float ClearLayerSpeed = 0.1f;
        private const float ClearedLayerDropSpeed = 0.25f;
        private const float DropNewShapeSpeed = 0.4f;

        public Shape shapeTemplate;
        public Block blockTemplate;
        public FallingBlock fallingBlockTemplate;

        public World world;
        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public AudioController audioController;

        public Vector3 Position { get; set; }
        public GameObject floor;
        public TMP_Text nameText, dropSpeedText;

        public ulong id;
        [SerializeField] private string containerName;
        private PlayerScore _score = new PlayerScore(0, 0);
        public bool dead;

        public Shape controllingShape;
        public List<PowerUp> PowerUps = new List<PowerUp>();

        private readonly Dictionary<Guid, Shape> _shapes = new Dictionary<Guid, Shape>();
        private readonly Dictionary<Guid, Block> _blocks = new Dictionary<Guid, Block>();

        public Vector3Int DropPosition { get; protected set; } = new Vector3Int(0, 20, 0);

        private int _dropSpeedMs = 1000;
        public const int DropSpeedFastMs = 10;
        private int DropSpeedIncrementMs => Math.Max(5, 40 / (world == null ? 1 : world.Containers.Count));

        public int MoveSpeedMs { get; } = 200;
        public int MoveResetSpeedMs { get; } = 50;

        public int RotateThreshold { get; } = 90;

        protected virtual void Start()
        {
            networkController.Client.RegisterListener(this);

            if (floor)
                floor.transform.localScale = new Vector3(Radius * 2 + 1, 1, Radius * 2 + 1);
            if (nameText)
                nameText.transform.position = new Vector3(nameText.transform.position.x, -2, -(Radius * 2 + 1) * 0.5f);
            if (dropSpeedText)
                dropSpeedText.transform.position = new Vector3(dropSpeedText.transform.position.x, -4, -(Radius * 2 + 1) * 0.5f);
        }

        private void OnDestroy()
        {
            networkController.Client.DeregisterListener(this);
        }

        private void Update()
        {
            if (dropSpeedText)
                dropSpeedText.text = Localization.Translate(TranslationKey.GameContainerDropSpeed, Math.Round((DropSpeedMs == 0 ? 100 : 1000f / DropSpeedMs) * 10) / 10);
        }

        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, Position, GameSettings.Settings.gameTransitionSpeed.FixedDelta() * 0.5f);
        }

        public Vector3Int GetRandomStartingPosition()
        {
            var x = Random.Range(-Radius, Radius);
            var z = Random.Range(-Radius, Radius);
            return DropPosition + new Vector3Int(x, 0, z);
        }
        
        public void StartDropping()
        {
            if (!IsDemo() && PowerUps.Count > 0)
            {
                var powerUp = PowerUps.LastOrDefault();
                if (powerUp != null)
                {
                    PowerUps.Remove(powerUp);
                    powerUp.Use(this);
                    return;
                }
            }
            
            StartDropping(IsDemo() ? (this as DemoContainer)?.GetNextOffsets() : null);
        }

        public void StartDropping((Guid, Vector3Int)[] offsets)
        {
            if ((gameController.ControllingContainer != this && !IsDemo()) || dead)
                return;

            offsets ??= ShapeUtil.Generate(networkController.Client.LobbyData.BlocksPerShape, networkController.Client.LobbyData.GenerateVerticalBlocks, GenerateBottomLeft, GenerateTopRight);

            if (!DoesCollide(offsets.Select((offset) => offset.Item2 + DropPosition).ToArray()))
            {
                var shape = CreateShape(Guid.NewGuid(), DropPosition, offsets, ColorUtil.GenerateColor());
                shape.StartDropping();
                controllingShape = shape;

                if (!IsDemo())
                    networkController.Client.SendPacket(new PacketShapeCreate
                    {
                        ContainerId = id,
                        Id = shape.ID,
                        Position = DropPosition,
                        Offsets = shape.Offsets,
                        Color = shape.BaseColor ?? Color.white,
                        Power = shape.PowerUp?.GetPower() ?? Power.None
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
                    networkController.Client.SendPacket(new PacketPlayerDead {Id = id});
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
                audioController.shapeLock.PlayModifiedSound(AudioController.GetGameVolume());
                
                networkController.Client.SendPacket(new PacketShapeLock
                {
                    Id = shape.ID,
                    Offsets = addedBlocks
                });
            }

            StartCoroutine(StartClearingLayers(addedBlocks));
        }

        private IEnumerator StartClearingLayers(Vector3Int[] addedBlocks, bool ignoreEmptySpaces = false)
        {
            yield return new WaitForSeconds(ClearLayerSpeed);

            var clearedLayers = ClearLayers(addedBlocks, ignoreEmptySpaces);

            if (clearedLayers.Count > 0)
                yield return DropLayers(clearedLayers);

            yield return new WaitForSeconds(DropNewShapeSpeed);

            if (!ignoreEmptySpaces)
                StartDropping();
        }
        
        private List<int> ClearLayers(IEnumerable<Vector3Int> addedBlocks, bool ignoreEmptySpaces = false)
        {
            var distinctLayers = addedBlocks.Select((vec) => vec.y).Distinct().ToArray();
            var clearingLayers = new List<int>();
            var deletedBlocks = new List<Guid>();
            foreach (var layer in distinctLayers)
            {
                var blocksInLayer = _blocks.Count((block) => block.Value.RawPosition.y == layer);
                var minBlockCountToClear = Math.Pow(Radius * 2 + 1, 2);
                if (blocksInLayer < minBlockCountToClear && !ignoreEmptySpaces)
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
                        Score = new PlayerScore(_score.Score + (deletedBlocks.Count * clearingLayers.Count), _score.ClearedLayers),
                    });
            }

            return clearingLayers;
        }

        private IEnumerator DropLayers(List<int> clearedLayers)
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
                audioController.layerDelete.PlayModifiedSound(AudioController.GetGameVolume());
                    
                networkController.Client.SendPacket(new PacketLayerMove
                {
                    ContainerId = id,
                    Layers = clearedLayers.ToArray()
                });
            }
        }

        private Shape CreateShape(Guid shapeId, Vector3Int position, (Guid, Vector3Int)[] offsets, Color? color = null, Power power = Power.None)
        {
            var shape = Instantiate(shapeTemplate, position, Quaternion.identity);
            shape.name = $"Shape-{shapeId}";

            shape.ID = shapeId;
            shape.Offsets = offsets;
            shape.BaseColor = color;
            if (!IsDemo())
                shape.PowerUp = PowerUpFactory.CreatePowerUp(power);

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
            block.parentContainer = this;

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

        public FallingBlock CreateFallingBlock(Guid blockId, Vector3Int position, Color color)
        {
            var block = Instantiate(fallingBlockTemplate, position, Quaternion.identity);
            block.name = $"FallingBlock-{blockId}";
            block.position = position;
            block.color = color;
            block.parentContainer = this;

            block.id = blockId;

            block.transform.SetParent(transform, false);
            
            return block;
        }

        private void RemoveBlock(Block block, int index = -1, int max = -1)
        {
            StartCoroutine(block.Remove(index, max));
            _blocks.Remove(block.id);
            if (!(block.parentShape is {PowerUp: { }} ps))
                return;

            if (!IsDemo() && gameController.ControllingContainer == this)
                PowerUps.Add(ps.PowerUp);
            ps.PowerUp = null;
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
                var color = ColorUtil.GenerateColor();

                CreateBlock(pos, color);
                score--;

                yield return new WaitForSeconds(0.075f);
            }
        }

        public Vector3Int GetDropToPosition(Vector3Int from)
        {
            var to = new Vector3Int(from.x, from.y, from.z);
            while (to.y > 1)
            {
                if (DoesCollide(new[] {to}))
                    break;
                to += Vector3Int.down;
            }
            return to;
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

            CreateShape(packet.Id, packet.Position, packet.Offsets, packet.Color, packet.Power);
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

        [PacketListener(PacketTypeId.LayerClear, PacketDirection.Client)]
        public void OnLayerClear(PacketLayerClear packet)
        {
            if (packet.ContainerId != id)
                return;

            StartCoroutine(StartClearingLayers(new[] {new Vector3Int(0, packet.Layer, 0)}, true));
        }

        [PacketListener(PacketTypeId.BlockBulkRemove, PacketDirection.Client)]
        public void OnBlockBulkRemove(PacketBlockBulkRemove packet)
        {
            if (packet.ContainerId != id)
                return;

            foreach (var blockId in packet.Ids)
                RemoveBlock(blockId);
        }

        [PacketListener(PacketTypeId.FallingBlockCreate, PacketDirection.Client)]
        public void OnFallingBlockCreate(PacketFallingBlockCreate packet)
        {
            if (packet.ContainerId != id)
                return;
            
            CreateFallingBlock(packet.Id, packet.Position, packet.Color);
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
        
        public int DropSpeedMs
        {
            get => IsDemo() ? 1000 : _dropSpeedMs;
            private set
            {
                if (_dropSpeedMs == value)
                    return;
                
                _dropSpeedMs = value;

                audioController.music.pitch = Mathf.Min(audioController.music.pitch + 0.01f, 2);
            }
        }
    }
}