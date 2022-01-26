using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Menu;
using Sabotris.Network;
using Sabotris.Network.Packets;
using Sabotris.Network.Packets.Game;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris
{
    public class Shape : MonoBehaviour
    {
        public Block blockTemplate;

        public GameController gameController;
        public MenuController menuController;
        public NetworkController networkController;
        public CameraController cameraController;
        public Container parentContainer;

        public Guid id;
        public Pair<Guid, Vector3Int>[] Offsets { get; set; }
        public Color? color = Color.white;
        
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
            
            foreach (var offset in Offsets)
                CreateBlock(offset.Key, offset.Value);

            foreach (var ren in GetComponentsInChildren<Renderer>())
                ren.material.color = color ?? Color.white;
            
            networkController.Client.RegisterListener(this);
        }

        private void OnDestroy()
        {
            networkController.Client.DeregisterListener(this);
        }

        private void Update()
        {
            if (parentContainer.controllingShape != this)
                return;

            DoMove();

            if (inputRotateYaw.Same(0, 1f)) inputRotateYaw = InputUtil.GetRotateYaw();
            if (inputRotatePitch.Same(0, 1f)) inputRotatePitch = InputUtil.GetRotatePitch();
            if (inputRotateRoll.Same(0, 1f)) inputRotateRoll = InputUtil.GetRotateRoll();
        }

        private void FixedUpdate()
        {
            if (parentContainer.controllingShape == this && !parentContainer.IsDemo() && !menuController.IsInMenu)
            {
                if (!InputUtil.ShouldRotateShape())
                    rotateActivator = RawRotation;
                
                DoRotate();

                var roundedRotationActivator =
                    Quaternion.Euler(rotateActivator.eulerAngles.Round(parentContainer.RotateThreshold));
                if (RawRotation != roundedRotationActivator)
                {
                    var offsets = GetOffsets(RawPosition, roundedRotationActivator)
                        ?.Select((offset) => offset.Value + RawPosition).ToArray();
                    if (!parentContainer.DoesCollide(offsets))
                        RawRotation = rotateActivator = roundedRotationActivator;
                    else
                        rotateActivator = RawRotation;
                }
            }
            
            transform.position = Vector3.Lerp(transform.position, parentContainer.transform.position + RawPosition, GameSettings.GameTransitionSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, RawRotation, GameSettings.GameTransitionSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, GameSettings.GameTransitionSpeed);
        }

        private void CreateBlock(Guid blockId, Vector3Int offset)
        {
            var block = Instantiate(blockTemplate, offset, Quaternion.identity);
            block.name = $"Block_{blockId}";

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
            _dropTimer.Reset();
            parentContainer.LockShape(this, Offsets.Select((offset) => RawPosition + offset.Value).ToArray());
        }
        
        private Pair<Guid, Vector3Int>[] GetOffsets(Vector3Int position, Quaternion rotation)
        {
            if (Offsets == null || Blocks.Count == 0)
                return null;
            
            var prevPosition = transform.position;
            var prevRotation = transform.rotation;

            transform.position = position;
            transform.rotation = rotation;
            Physics.SyncTransforms();

            var offsets = Blocks.Values.Select(block => new Pair<Guid, Vector3Int>(block.id, Vector3Int.RoundToInt(block.transform.position - position))).ToArray();

            transform.position = prevPosition;
            transform.rotation = prevRotation;
            Physics.SyncTransforms();

            return offsets;
        }

        private void DoMove()
        {
            var moveVec = Vector3.zero;

            var isDropping = false;
            if (_dropTimer.ElapsedMilliseconds > (InputUtil.GetMoveDown() && !parentContainer.IsDemo() && !menuController.IsInMenu ? parentContainer.DropSpeedFastMs : parentContainer.DropSpeedMs))
            {
                _dropTimer.Restart();
                isDropping = true;
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
                    where !parentContainer.DoesCollide(Offsets.Select((offset) => offset.Value + RawPosition + rounded).ToArray())
                    select vec
                ).Aggregate(moveVec, (a, b) => a + b);
            }

            var roundedMoveVec = Vector3Int.RoundToInt(moveVec);
            if (roundedMoveVec.y == 0 && isDropping)
                StopDropping();
            else if (roundedMoveVec != Vector3Int.zero)
            {
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

            transform.position = RawPosition;
            transform.rotation = rotateActivator;
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
            Physics.SyncTransforms();
        }

        [PacketListener(PacketTypeId.ShapeMove, PacketDirection.Client)]
        public void OnShapeMove(PacketShapeMove packet)
        {
            if (packet.Id != id)
                return;

            RawPosition = packet.Position;
        }

        [PacketListener(PacketTypeId.ShapeRotate, PacketDirection.Client)]
        public void OnShapeRotate(PacketShapeRotate packet)
        {
            if (packet.Id != id)
                return;

            RawRotation = packet.Rotation;
        }

        [PacketListener(PacketTypeId.ShapeLock, PacketDirection.Client)]
        public void OnShapeLock(PacketShapeLock packet)
        {
            if (packet.Id != id)
                return;

            parentContainer.LockShape(this, packet.Offsets.ToArray());
        }

        public Vector3Int RawPosition
        {
            get => rawPosition;
            set
            {
                if (RawPosition == value) return;

                if (!parentContainer.DoesCollide(Offsets.Select((offset) => offset.Value + value).ToArray()))
                {
                    rawPosition = value;
                    if (parentContainer.controllingShape == this)
                    {
                        networkController.Client.SendPacket(new PacketShapeMove
                        {
                            Id = id,
                            Position = RawPosition
                        });
                    }
                }
            }
        }
        
        public Quaternion RawRotation
        {
            get => rawRotation;
            set
            {
                if (RawRotation == value) return;
                
                rawRotation = value;
                
                var offsets = GetOffsets(RawPosition, rawRotation);
                if (offsets != null)
                    Offsets = offsets;
                
                if (parentContainer.controllingShape == this)
                    networkController.Client.SendPacket(new PacketShapeRotate
                    {
                        Id = id,
                        Rotation = RawRotation
                    });
            }
        }
    }
}
