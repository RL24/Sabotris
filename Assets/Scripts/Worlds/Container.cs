using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sabotris.Audio;
using Sabotris.Game;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Block;
using Sabotris.Network.Packets.Game;
using Sabotris.Network.Packets.Layer;
using Sabotris.Network.Packets.Players;
using Sabotris.Network.Packets.Shape;
using Sabotris.Powers;
using Sabotris.Translations;
using Sabotris.UI;
using Sabotris.UI.Menu;
using Sabotris.Util;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = Sabotris.Util.Random;

namespace Sabotris.Worlds
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
        public event EventHandler<PowerUp> OnAddPowerUp;
        public event EventHandler<PowerUp> OnRemovePowerUp;
        
        private const float AddLayerSpeed = 0.1f;
        private const float AddedLayerRaiseSpeed = 0.25f;
        private const float ClearLayerSpeed = 0.1f;
        private const float ClearedLayerDropSpeed = 0.25f;
        private const float DropNewShapeSpeed = 0.4f;
        private const int PowerUpCountDelay = 10;
        
        protected int Radius => (networkController ? networkController.Client?.LobbyData?.PlayFieldSize : null) ?? 2;
        private Vector3Int BottomLeft => new Vector3Int(-Radius, 1, -Radius);
        private Vector3Int TopRight => new Vector3Int(Radius, 25, Radius);

        public Vector3Int GenerateBottomLeft => new Vector3Int(-Radius, 0, -Radius);
        public Vector3Int GenerateTopRight => new Vector3Int(Radius, 24, Radius);

        public Shape shapeTemplate;
        public FallingShape fallingShapeTemplate;
        public Block blockTemplate;
        public FallingBlock fallingBlockTemplate;

        public World world;
        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public AudioController audioController;
        public SelectorOverlay selectorOverlay;

        public Vector3 rawPosition;
        public GameObject floor;
        public TMP_Text nameText, dropSpeedText;

        public Guid Id;
        public ulong steamId;
        [SerializeField] private string containerName;
        private PlayerScore _score = new PlayerScore(0, 0);
        public bool dead;

        private Shape _controllingShape;
        private readonly List<PowerUp> _powerUps = new List<PowerUp>();
        public readonly Stopwatch PowerUpTimer = new Stopwatch();
        private int _powerUpCount;

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
        }

        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, rawPosition, GameSettings.Settings.gameTransitionSpeed.FixedDelta() * 0.5f);
        }

        public Vector3Int GetRandomStartingPosition((Guid, Vector3Int)[] offsets = null)
        {
            var minX = offsets?.Min((offset) => offset.Item2.x) ?? 0;
            var maxX = offsets?.Max((offset) => offset.Item2.x) ?? 0;
            var minZ = offsets?.Min((offset) => offset.Item2.z) ?? 0;
            var maxZ = offsets?.Max((offset) => offset.Item2.z) ?? 0;
            var x = Random.Range(-Radius - minX, Radius - maxX);
            var z = Random.Range(-Radius - minZ, Radius - maxZ);
            return DropPosition + new Vector3Int(x, 0, z);
        }
        
        public void StartDropping()
        {
            if (dead || this is BotContainer && networkController.Server?.Running == false)
                return;

            if (!(this is DemoContainer) && _powerUps.Count > 0)
            {
                var powerUp = _powerUps.LastOrDefault();
                if (powerUp != null)
                {
                    _powerUps.Remove(powerUp);
                    OnRemovePowerUp?.Invoke(this, powerUp);
                    selectorOverlay.Open(powerUp, PowerUpTimer);
                    PowerUpTimer.Restart();
                    powerUp.Use(this);
                    return;
                }
            }

            var blocksPerShape = (networkController ? networkController.Client?.LobbyData.BlocksPerShape : null) ?? 4;
            var generateVerticalBlocks = (networkController ? networkController.Client?.LobbyData.GenerateVerticalBlocks : null) ?? false;

            var offsets = ShapeUtil.Generate(blocksPerShape, generateVerticalBlocks, GenerateBottomLeft, GenerateTopRight);

            if (!DoesCollide(offsets.Select((offset) => offset.Item2 + DropPosition).ToArray()))
            {
                var shape = CreateShape(Guid.NewGuid(), DropPosition, offsets, Random.RandomColor());
                shape.StartDropping();
                ControllingShape = shape;

                if (ShouldSendPacket())
                    networkController.Client?.SendPacket(new PacketShapeCreate
                    {
                        ContainerId = Id,
                        Id = shape.Id,
                        Position = DropPosition,
                        Offsets = shape.Offsets,
                        Color = shape.ShapeColor ?? Color.white,
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

                if (ShouldSendPacket())
                    networkController.Client?.SendPacket(new PacketPlayerDead {Id = Id});
            }
        }

        public void LockShape(Shape shape, Vector3Int[] addedBlocks)
        {
            shape.locked = true;
            ControllingShape = null;

            foreach (var blockEntry in shape.Blocks)
            {
                var blockId = blockEntry.Key;
                var block = blockEntry.Value;
                _blocks.Add(blockId, block);
                block.RawPosition = shape.RawPosition + shape.Offsets.First((pair) => pair.Item1 == blockId).Item2;
                block.shifted = true;
            }

            if (gameController.ControllingContainer != this && !(this is BotContainer && networkController.Server?.Running == true || this is DemoContainer))
                return;

            if (audioController)
                audioController.shapeLock.PlayModifiedSound(AudioController.GetGameVolume());
            if (ShouldSendPacket())
                networkController.Client?.SendPacket(new PacketShapeLock
                {
                    Id = shape.Id,
                    LockPos = shape.RawPosition,
                    LockRot = shape.RawRotation,
                    Offsets = addedBlocks
                });

            StartCoroutine(StartClearingLayers(addedBlocks));
        }

        public IEnumerator StartClearingLayers(Vector3Int[] addedBlocks, bool ignoreEmptySpaces = false)
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
            var powerUpsPerLayer = new Dictionary<int, List<PowerUp>>();
            foreach (var layer in distinctLayers)
            {
                var blocksInLayer = _blocks.Count((block) => block.Value.RawPosition.y == layer);
                var minBlockCountToClear = Math.Pow(Radius * 2 + 1, 2);
                if (blocksInLayer < minBlockCountToClear && !ignoreEmptySpaces)
                    continue;

                var blocksToRemove = _blocks.Where((block) => block.Value.RawPosition.y == layer).Select((block) => block.Value).ToArray();
                foreach (var block in blocksToRemove)
                {
                    deletedBlocks.Add(block.Id);
                    var (powerUp, powerUpLayer) = RemoveBlock(block);
                    if (powerUp == null)
                        continue;
                    
                    if (!powerUpsPerLayer.ContainsKey(powerUpLayer))
                        powerUpsPerLayer.Add(powerUpLayer, new List<PowerUp> {powerUp});
                    powerUpsPerLayer[powerUpLayer].Add(powerUp);
                }

                clearingLayers.Add(layer);
            }

            if (!(this is DemoContainer) && gameController.ControllingContainer == this)
            {
                foreach (var powerUpEntries in powerUpsPerLayer)
                {
                    var powerUp = powerUpEntries.Value[Random.Range(0, powerUpEntries.Value.Count - 1)];
                    _powerUps.Add(powerUp);
                    OnAddPowerUp?.Invoke(this, powerUp);
                }
            }

            if (ShouldSendPacket())
            {
                if (deletedBlocks.Any())
                    networkController.Client?.SendPacket(new PacketBlockBulkRemove
                    {
                        ContainerId = Id,
                        Ids = deletedBlocks.ToArray()
                    });

                if (clearingLayers.Any())
                    networkController.Client?.SendPacket(new PacketPlayerScore
                    {
                        Id = Id,
                        Score = new PlayerScore(_score.Score + (deletedBlocks.Count * clearingLayers.Count), _score.ClearedLayers)
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

            if (audioController)
                audioController.layerDelete.PlayModifiedSound(AudioController.GetGameVolume());
            if (ShouldSendPacket())
                networkController.Client?.SendPacket(new PacketLayerMove
                {
                    ContainerId = Id,
                    Layers = clearedLayers.ToArray(),
                    Y = Vector3Int.down.y
                });
        }

        private void RaiseLayer(int layer)
        {
            if (_controllingShape)
                _controllingShape.RawPosition += Vector3Int.up;

            foreach (var block in _blocks.Values.Where((block) => block.RawPosition.y >= layer))
            {
                block.RawPosition += Vector3Int.up;
                block.shifted = true;
            }

            if (ShouldSendPacket())
                networkController.Client?.SendPacket(new PacketLayerMove
                {
                    ContainerId = Id,
                    Layers = new[] {layer - 1},
                    Y = Vector3Int.up.y
                });
        }

        public IEnumerator AddLayer(int layer)
        {
            yield return new WaitForSeconds(AddLayerSpeed);
            
            RaiseLayer(layer);

            yield return new WaitForSeconds(AddedLayerRaiseSpeed);

            var color = Random.RandomColor();
            var blocks = new List<(Guid, Vector3Int, Color)>();
            for (var x = -Radius; x <= Radius; x++)
                for (var z = -Radius; z <= Radius; z++)
                {
                    var block = (Guid.NewGuid(), new Vector3Int(x, layer, z), color);
                    CreateBlock(block.Item1, block.Item2, block.Item3);
                    blocks.Add(block);
                }

            if (ShouldSendPacket())
                networkController.Client?.SendPacket(new PacketBlockBulkCreate
                {
                    ContainerId = Id,
                    Blocks = blocks.ToArray()
                });

            yield return new WaitForSeconds(DropNewShapeSpeed);
        }

        private Shape CreateShape(Guid shapeId, Vector3Int position, (Guid, Vector3Int)[] offsets, Color? color = null, Power? power = null)
        {
            var shape = Instantiate(shapeTemplate, position, Quaternion.identity);
            shape.name = $"Shape-{shapeId}";

            shape.Id = shapeId;
            shape.Offsets = offsets;
            shape.ShapeColor = color;

            if (!(this is DemoContainer || this is BotContainer) && world.Containers.Count > 1 && _powerUpCount > PowerUpCountDelay) {
                shape.PowerUp = PowerUpFactory.CreatePowerUp(power);
                if (shape.PowerUp != null)
                    _powerUpCount = 0;
            }
            if (power == null && shape.PowerUp == null)
                _powerUpCount++;

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
        
        public FallingShape CreateFallingShape(Guid shapeId, Vector3Int position, (Guid, Vector3Int)[] offsets, Color? color = null)
        {
            var shape = Instantiate(fallingShapeTemplate, position, Quaternion.identity);
            shape.name = $"Shape-{shapeId}";

            shape.Id = shapeId;
            shape.RawPosition = position;
            shape.Offsets = offsets;
            shape.ShapeColor = color;

            shape.gameController = gameController;
            shape.networkController = networkController;
            shape.parentContainer = this;

            shape.transform.SetParent(transform, false);

            return shape;
        }

        private void CreateBlock(Vector3Int position, Color color)
        {
            CreateBlock(Guid.NewGuid(), position, color);
        }

        public void CreateBlock(Guid blockId, Vector3Int position, Color color)
        {
            var block = Instantiate(blockTemplate, position, Quaternion.identity);
            block.name = $"Block-{blockId}";
            
            block.Id = blockId;
            block.RawPosition = position;
            block.BlockColor = color;
            
            block.parentContainer = this;
            
            block.transform.SetParent(transform, false);
            block.transform.localScale = Vector3.zero;

            _blocks.Add(blockId, block);
        }

        public FallingBlock CreateFallingBlock(Guid blockId, Vector3Int position, Color color)
        {
            var block = Instantiate(fallingBlockTemplate, position, Quaternion.identity);
            block.name = $"FallingBlock-{blockId}";
            
            block.Id = blockId;
            block.RawPosition = position;
            block.BlockColor = color;

            block.gameController = gameController;
            block.networkController = networkController;
            block.parentContainer = this;

            block.transform.SetParent(transform, false);

            return block;
        }
        
        private void RemoveBlock(Guid blockId, int index = -1, int max = -1)
        {
            if (!_blocks.TryGetValue(blockId, out var block))
                return;

            RemoveBlock(block, index, max);
        }

        private (PowerUp, int) RemoveBlock(Block block, int index = -1, int max = -1)
        {
            StartCoroutine(block.Remove(index, max));
            _blocks.Remove(block.Id);

            if (!(block.parentShape is {PowerUp: { }} ps))
                return (null, -1);

            var powerUp = ps.PowerUp;
            ps.PowerUp = null;
            return (powerUp, block.RawPosition.y);
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
                var color = Random.RandomColor();

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
                if (DoesCollide(new[] {to + Vector3Int.down}))
                    break;
                to += Vector3Int.down;
            }
            return to;
        }

        public void SelectedContainer()
        {
            selectorOverlay.Close();
            PowerUpTimer.Stop();
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
            if (packet.ContainerId != Id)
                return;

            CreateShape(packet.Id, packet.Position, packet.Offsets, packet.Color, packet.Power);
        }
        
        [PacketListener(PacketTypeId.FallingShapeCreate, PacketDirection.Client)]
        public void OnFallingShapeCreate(PacketFallingShapeCreate packet)
        {
            if (packet.ContainerId != Id)
                return;

            CreateFallingShape(packet.Id, packet.Position, packet.Offsets, packet.Color);
        }

        [PacketListener(PacketTypeId.LayerMove, PacketDirection.Client)]
        public void OnLayerMove(PacketLayerMove packet)
        {
            if (packet.ContainerId != Id)
                return;

            foreach (var layer in packet.Layers)
            {
                if (packet.Y == Vector3Int.up.y)
                    if (_controllingShape)
                        _controllingShape.RawPosition += new Vector3Int(0, packet.Y, 0);
                
                foreach (var block in _blocks.Values.Where((block) => block.RawPosition.y > layer))
                {
                    block.RawPosition += new Vector3Int(0, packet.Y, 0);
                    block.shifted = true;
                }
            }
        }

        [PacketListener(PacketTypeId.LayerClear, PacketDirection.Client)]
        public void OnLayerClear(PacketLayerClear packet)
        {
            if (packet.ContainerId != Id)
                return;

            StartCoroutine(StartClearingLayers(new[] {new Vector3Int(0, packet.Layer, 0)}, true));
        }

        [PacketListener(PacketTypeId.LayerClear, PacketDirection.Client)]
        public void OnLayerAdd(PacketLayerAdd packet)
        {
            if (packet.ContainerId != Id)
                return;

            StartCoroutine(AddLayer(packet.Layer));
        }
        
        [PacketListener(PacketTypeId.BlockBulkCreate, PacketDirection.Client)]
        public void OnBlockBulkCreate(PacketBlockBulkCreate packet)
        {
            if (packet.ContainerId != Id)
                return;

            foreach (var (id, pos, color) in packet.Blocks)
                CreateBlock(id, pos, color);
        }
        
        [PacketListener(PacketTypeId.BlockBulkRemove, PacketDirection.Client)]
        public void OnBlockBulkRemove(PacketBlockBulkRemove packet)
        {
            if (packet.ContainerId != Id)
                return;

            foreach (var blockId in packet.Ids)
                RemoveBlock(blockId);
        }
        
        [PacketListener(PacketTypeId.BlockCreate, PacketDirection.Client)]
        public void OnBlockCreate(PacketBlockCreate packet)
        {
            if (packet.ContainerId != Id)
                return;

            CreateBlock(packet.Id, packet.Position, packet.Color);
        }
        
        [PacketListener(PacketTypeId.FallingBlockCreate, PacketDirection.Client)]
        public void OnFallingBlockCreate(PacketFallingBlockCreate packet)
        {
            if (packet.ContainerId != Id)
                return;

            CreateFallingBlock(packet.Id, packet.Position, packet.Color);
        }

        [PacketListener(PacketTypeId.PlayerDead, PacketDirection.Client)]
        public void OnPlayerDead(PacketPlayerDead packet)
        {
            if (packet.Id != Id)
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

            if (packet.Id != Id)
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

        protected virtual bool ShouldSendPacket()
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
            private set
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