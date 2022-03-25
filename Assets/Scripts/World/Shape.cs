using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sabotris.Audio;
using Sabotris.IO;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Powers;
using Sabotris.UI.Menu;
using Sabotris.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sabotris
{
    public class Shape : MonoBehaviour
    {
        private static readonly Color PowerUpColor = Color.HSVToRGB(0, 0, 0.2f);
        
        public Block blockTemplate;

        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public AudioController audioController;
        public Container parentContainer;

        public Guid ID;
        public (Guid, Vector3Int)[] Offsets { get; set; }
        public Color? BaseColor = Color.white;
        public bool locked;
        private PowerUp _powerUp;

        [SerializeField] private Vector3Int rawPosition;
        [SerializeField] private Quaternion rawRotation;
        [SerializeField] private Quaternion rotateActivator;

        private readonly Stopwatch _dropTimer = new Stopwatch(),
            _moveTimer = new Stopwatch(),
            _moveResetTimer = new Stopwatch();

        private float _prevAdvance, _prevStrafe;

        [SerializeField] private float inputRotateYaw;
        [SerializeField] private float inputRotatePitch;
        [SerializeField] private float inputRotateRoll;

        public readonly Dictionary<Guid, Block> Blocks = new Dictionary<Guid, Block>();

        private void Start()
        {
            RawPosition = Vector3Int.RoundToInt(transform.position - parentContainer.transform.position);
            RawRotation = transform.rotation;

            transform.localScale = Vector3.zero;

            foreach (var (blockId, blockPos) in Offsets)
                CreateBlock(blockId, blockPos);

            var color = PowerUp == null ? (BaseColor ?? Color.white) : PowerUpColor;
            foreach (var ren in GetComponentsInChildren<Renderer>())
                ren.material.color = color;

            networkController.Client.RegisterListener(this);
        }

        private void OnDestroy()
        {
            networkController.Client.DeregisterListener(this);
        }

        private void Update()
        {
            if (!IsControlling())
                return;

            DoMove();

            if (inputRotateYaw.Same(0, 1f)) inputRotateYaw = InputUtil.GetRotateYaw();
            if (inputRotatePitch.Same(0, 1f)) inputRotatePitch = InputUtil.GetRotatePitch();
            if (inputRotateRoll.Same(0, 1f)) inputRotateRoll = InputUtil.GetRotateRoll();
            
            var color = PowerUp == null ? (BaseColor ?? Color.white) : PowerUpColor;
            foreach (var ren in GetComponentsInChildren<Renderer>())
                ren.material.color = Color.Lerp(ren.material.color, color, Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (IsControlling() && !parentContainer.IsDemo() && !menuController.IsInMenu)
            {
                if (!InputUtil.ShouldRotateShape())
                    rotateActivator = RawRotation;

                DoRotate();

                var roundedRotationActivator = Quaternion.Euler(rotateActivator.eulerAngles.Round(parentContainer.RotateThreshold));
                if (!RawRotation.eulerAngles.Same(roundedRotationActivator.eulerAngles))
                {
                    var offsets = GetOffsets(RawPosition, roundedRotationActivator)?.Select((offset) => offset.Item2 + RawPosition).ToArray();
                    if (!parentContainer.DoesCollide(offsets))
                    {
                        RawRotation = rotateActivator = roundedRotationActivator;
                        audioController.shapeRotate.PlayModifiedSound(AudioController.GetGameVolume(), AudioController.GetShapeRotatePitch());
                    }
                    else
                        rotateActivator = RawRotation;
                }
            }

            transform.position = Vector3.Lerp(transform.position, parentContainer.transform.position + RawPosition, GameSettings.Settings.gameTransitionSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, RawRotation, GameSettings.Settings.gameTransitionSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, GameSettings.Settings.gameTransitionSpeed);
        }

        private void CreateBlock(Guid blockId, Vector3Int offset)
        {
            var block = Instantiate(blockTemplate, offset, Quaternion.identity);
            block.name = $"Block-{blockId}";

            block.parentContainer = parentContainer;
            block.parentShape = this;
            block.id = blockId;

            block.transform.SetParent(transform, false);

            Blocks.Add(blockId, block);
        }

        public void RemoveBlock(Guid blockId)
        {
            if (Blocks.TryGetValue(blockId, out var block))
                Destroy(block.gameObject);
            Blocks.Remove(blockId);
        }

        public void StartDropping()
        {
            if (!_dropTimer.IsRunning)
                _dropTimer.Start();
            else _dropTimer.Restart();
        }

        private void StopDropping()
        {
            locked = true;
            _dropTimer.Reset();
            parentContainer.LockShape(this, Offsets.Select((offset) => RawPosition + offset.Item2).ToArray());
        }

        private (Guid, Vector3Int)[] GetOffsets(Vector3Int position, Quaternion rotation)
        {
            if (Offsets == null || Blocks.Count == 0)
                return null;

            var prevPosition = transform.position;
            var prevRotation = transform.rotation;
            var prevScale = transform.localScale;

            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = Vector3.one;
            Physics.SyncTransforms();

            var offsets = Blocks.Values.Select(block => (block.id, Vector3Int.RoundToInt(block.transform.position - position))).ToArray();

            transform.position = prevPosition;
            transform.rotation = prevRotation;
            transform.localScale = prevScale;
            Physics.SyncTransforms();

            return offsets;
        }

        private void DoMove()
        {
            var moveVec = Vector3.zero;

            var doFastMoveDown = InputUtil.GetMoveDown();
            var isDropping = false;
            if (_dropTimer.ElapsedMilliseconds > (doFastMoveDown && !parentContainer.IsDemo() && !menuController.IsInMenu ? Container.DropSpeedFastMs : parentContainer.DropSpeedMs))
            {
                _dropTimer.Restart();
                isDropping = !networkController.Client.LobbyData.PracticeMode || doFastMoveDown || parentContainer.IsDemo();
            }

            var advance = InputUtil.GetMoveAdvance();
            var strafe = InputUtil.GetMoveStrafe();

            if (parentContainer.IsDemo() || menuController.IsInMenu)
                advance = strafe = 0;

            if (((advance == 0 && strafe == 0) || !_prevAdvance.Same(advance, 0.5f) || !_prevStrafe.Same(strafe, 0.5f) ||
                 _moveTimer.ElapsedMilliseconds > parentContainer.MoveSpeedMs) && _moveTimer.IsRunning && !_moveResetTimer.IsRunning)
                _moveResetTimer.Start();

            if (_moveTimer.IsRunning && _moveResetTimer.ElapsedMilliseconds > parentContainer.MoveResetSpeedMs)
            {
                _moveResetTimer.Reset();
                _moveTimer.Reset();
            }

            _prevAdvance = advance;
            _prevStrafe = strafe;

            if (_moveTimer.IsRunning)
                advance = strafe = 0;

            var advanceDir = advance == 0
                ? Vector3.zero
                : Quaternion.AngleAxis(cameraController.Yaw, Vector3.up) * Vector3Int.back;

            var strafeDir = strafe == 0
                ? Vector3.zero
                : Quaternion.AngleAxis(cameraController.Yaw + 90, Vector3.up) * Vector3Int.forward;

            var isMoving = advance != 0 || strafe != 0;
            if (isMoving || isDropping)
            {
                var vecs = new List<Vector3>();

                if (isMoving)
                {
                    vecs.Add(new Vector3(advanceDir.x * advance + strafeDir.x * strafe, 0, 0));
                    vecs.Add(new Vector3(0, 0, advanceDir.z * advance + strafeDir.z * strafe));
                }

                if (isDropping)
                    vecs.Add(Vector3.down);

                moveVec = (
                    from vec in vecs
                    let rounded = Vector3Int.RoundToInt(vec)
                    where !parentContainer.DoesCollide(Offsets.Select((offset) => offset.Item2 + RawPosition + rounded).ToArray())
                    select vec
                ).Aggregate(moveVec, (a, b) => a + b);
            }

            var roundedMoveVec = Vector3Int.RoundToInt(moveVec);
            if (roundedMoveVec.y == 0 && isDropping)
                StopDropping();
            else if (roundedMoveVec != Vector3Int.zero)
            {
                if (IsControlling() && !parentContainer.IsDemo())
                {
                    var sound = isDropping ? audioController.shapeDrop : audioController.shapeMove;
                    if (isDropping && doFastMoveDown && !sound.isPlaying || isDropping && !doFastMoveDown || !isDropping)
                        sound.PlayModifiedSound(AudioController.GetGameVolume(), AudioController.GetShapeMovePitch());
                }
                
                RawPosition += roundedMoveVec;
                if (roundedMoveVec.x != 0 || roundedMoveVec.z != 0)
                    _moveTimer.Start();
            }
        }

        private void DoRotate()
        {
            var inputYaw = inputRotateYaw;
            var inputPitch = inputRotatePitch;
            var inputRoll = inputRotateRoll;

            inputRotateYaw = inputRotatePitch = inputRotateRoll = 0;

            if (Math.Abs(inputYaw) < 1 && Math.Abs(inputPitch) < 1 && Math.Abs(inputRoll) < 1)
                return;

            var prevPosition = transform.position;
            var prevRotation = transform.rotation;
            var prevScale = transform.localScale;

            transform.position = RawPosition;
            transform.rotation = rotateActivator;
            transform.localScale = Vector3.one;
            Physics.SyncTransforms();

            var pitchAxis = Quaternion.AngleAxis(cameraController.Yaw, Vector3.up) * Vector3Int.right;
            var rollAxis = Quaternion.AngleAxis(cameraController.Yaw, Vector3.up) * Vector3Int.back;
            transform.RotateAround(RawPosition, Vector3.up, inputYaw);
            transform.RotateAround(RawPosition, pitchAxis, inputPitch);
            transform.RotateAround(RawPosition, rollAxis, inputRoll);
            Physics.SyncTransforms();

            rotateActivator = transform.rotation;

            transform.position = prevPosition;
            transform.rotation = prevRotation;
            transform.localScale = prevScale;
            Physics.SyncTransforms();
        }

        [PacketListener(PacketTypeId.ShapeMove, PacketDirection.Client)]
        public void OnShapeMove(PacketShapeMove packet)
        {
            if (packet.Id != ID)
                return;

            RawPosition = packet.Position;
        }

        [PacketListener(PacketTypeId.ShapeRotate, PacketDirection.Client)]
        public void OnShapeRotate(PacketShapeRotate packet)
        {
            if (packet.Id != ID)
                return;

            RawRotation = packet.Rotation;
        }

        [PacketListener(PacketTypeId.ShapeLock, PacketDirection.Client)]
        public void OnShapeLock(PacketShapeLock packet)
        {
            if (packet.Id != ID)
                return;

            locked = true;
            parentContainer.LockShape(this, packet.Offsets.ToArray());
        }
        
        private bool IsControlling() => parentContainer.controllingShape == this;

        public Vector3Int RawPosition
        {
            get => rawPosition;
            private set
            {
                if (RawPosition == value) return;

                if (!parentContainer.DoesCollide(Offsets.Select((offset) => offset.Item2 + value).ToArray()))
                {
                    rawPosition = value;
                    if (IsControlling())
                    {
                        networkController.Client.SendPacket(new PacketShapeMove
                        {
                            Id = ID,
                            Position = RawPosition
                        });
                    }
                }
            }
        }

        private Quaternion RawRotation
        {
            get => rawRotation;
            set
            {
                if (RawRotation == value) return;

                rawRotation = value;

                var offsets = GetOffsets(RawPosition, rawRotation);
                if (offsets != null)
                    Offsets = offsets;

                if (IsControlling())
                    networkController.Client.SendPacket(new PacketShapeRotate
                    {
                        Id = ID,
                        Rotation = RawRotation
                    });
            }
        }

        public PowerUp PowerUp
        {
            get => _powerUp;
            set
            {
                if (value == PowerUp)
                    return;

                _powerUp = value;
            }
        }
    }
}