using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sabotris.Util;
using UnityEngine;
using Random = Sabotris.Util.Random;

namespace Sabotris
{
    public class ControlledContainer : Container
    {
        private static readonly Quaternion[] Rotations = GenerateRotations();

        private static Quaternion[] GenerateRotations()
        {
            var rotations = new List<Quaternion>();
            for (var i = 0; i <= 270; i += 90)
            for (var j = 0; j <= 270; j += 90)
            for (var k = 0; k <= 270; k += 90)
                rotations.Add(Quaternion.Euler(i, j, k));
            return rotations.ToArray();
        }

        private (Vector3Int?, Quaternion) _destination;

        private List<(Vector3Int[], Quaternion)> _offsets = new List<(Vector3Int[], Quaternion)>();

        private IEnumerable<Vector3Int> GetEmptySpaces()
        {
            var emptySpaces = new List<Vector3Int>();
            for (var x = -Radius; x <= Radius; x++)
            {
                for (var z = -Radius; z <= Radius; z++)
                {
                    for (var y = DropPosition.y; y > 0; y--)
                    {
                        var vec = new Vector3Int(x, y - 1, z);
                        if (DoesCollide(new[] {vec}))
                        {
                            emptySpaces.Add(new Vector3Int(x, y, z));
                            break;
                        }
                    }
                }
            }

            return emptySpaces.ToArray();
        }

        private void GetNextSpaceAndRotation()
        {
            var spaces = GetEmptySpaces();
            var availableSpaces = new List<(Vector3Int, Quaternion, Vector3Int[])>();
            foreach (var space in spaces)
            foreach (var (offset, rotation) in _offsets)
                for (var y = 0; y <= (int) Math.Floor(offset.Size().y / 2f); y++)
                {
                    var relativeOffset = offset.RelativeTo(space + (Vector3Int.up * y));
                    if (!DoesCollide(relativeOffset) && !IsBlockingSpace(relativeOffset))
                    {
                        availableSpaces.Add((space, rotation, offset));
                        break;
                    }
                }

            availableSpaces = availableSpaces.OrderBy((space) =>
            {
                var shapeMin = space.Item3.Min((vec) => vec.y);
                return space.Item1.y + shapeMin;
            }).ToList();

            if (availableSpaces.Count <= 0)
            {
                var (offsets, rotation) = _offsets[Random.Range(0, _offsets.Count - 1)];
                var shapeMin = offsets.MinVec();
                var shapeMax = offsets.MaxVec();
                var min = new Vector3Int(-Radius, 0, -Radius) - shapeMin;
                var max = new Vector3Int(Radius, 0, Radius) - shapeMax;
                _destination = (new Vector3Int(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z)), rotation);
                return;
            }

            var lowestOptions = availableSpaces.Where((space) => space.Item1.y == availableSpaces[0].Item1.y).ToArray();
            var (bestSpace, bestRotation, _) = lowestOptions[Random.Range(0, lowestOptions.Length - 1)];
            _destination = (bestSpace, bestRotation);
        }

        private bool IsBlockingSpace(Vector3Int[] offset)
        {
            foreach (var pos in offset)
                if (!DoesCollide(new[] {pos + Vector3Int.down}) && offset.All(vec => vec != pos + Vector3Int.down))
                    return true;

            return false;
        }

        private IEnumerator GoToDestination(Shape shape)
        {
            yield return new WaitUntil(() => shape && shape.Blocks.Count > 0);
            yield return new WaitForSeconds(GetScanDelay());

            _offsets = new List<(Vector3Int[], Quaternion)>();
            var prevRotation = shape.RawRotation;
            foreach (var rot in Rotations)
            {
                shape.RawRotation = rot;
                _offsets.Add((shape.Offsets.Select((offset) => offset.Item2.Copy()).ToArray(), rot));
            }

            shape.RawRotation = prevRotation;

            _destination = (null, Quaternion.identity);

            new Thread(GetNextSpaceAndRotation).Start();

            yield return new WaitUntil(() => _destination.Item1 != null);

            var position = _destination.Item1;
            var rotation = _destination.Item2.eulerAngles;

            if (position == null)
                yield break;

            while (shape && !shape.locked
                         && (shape.RawPosition.x != position.Value.x
                             || shape.RawPosition.z != position.Value.z
                             || !shape.RawRotation.eulerAngles.x.Same(rotation.x)
                             || !shape.RawRotation.eulerAngles.y.Same(rotation.y)
                             || !shape.RawRotation.eulerAngles.z.Same(rotation.z)))
            {
                yield return new WaitForSeconds(Random.Range(GetMinimumMoveDelay(), GetMaximumMoveDelay()));

                if (!shape || shape.locked)
                    yield break;

                var choices = new List<Movement>();
                if (!shape.RawRotation.eulerAngles.x.Same(rotation.x))
                    choices.Add(Movement.RotateX);
                if (!shape.RawRotation.eulerAngles.y.Same(rotation.y))
                    choices.Add(Movement.RotateY);
                if (!shape.RawRotation.eulerAngles.z.Same(rotation.z))
                    choices.Add(Movement.RotateZ);
                if (choices.Count == 0)
                {
                    if (shape.RawPosition.x != position.Value.x)
                        choices.Add(Movement.X);
                    if (shape.RawPosition.z != position.Value.z)
                        choices.Add(Movement.Z);
                }

                if (choices.Count == 0)
                    break;

                var positionDirection = Vector3.Normalize(position.Value - shape.RawPosition);

                switch (choices[Random.Range(0, choices.Count - 1)])
                {
                    case Movement.X:
                        shape.RawPosition += Vector3Int.right * Math.Sign(positionDirection.x);
                        break;
                    case Movement.Z:
                        shape.RawPosition += Vector3Int.forward * Math.Sign(positionDirection.z);
                        break;
                    case Movement.RotateX:
                        shape.rotateActivator = Quaternion.Euler(rotation.x, shape.rotateActivator.eulerAngles.y, shape.rotateActivator.eulerAngles.z);
                        break;
                    case Movement.RotateY:
                        shape.rotateActivator = Quaternion.Euler(shape.rotateActivator.eulerAngles.x, rotation.y, shape.rotateActivator.eulerAngles.z);
                        break;
                    case Movement.RotateZ:
                        shape.rotateActivator = Quaternion.Euler(shape.rotateActivator.eulerAngles.x, shape.rotateActivator.eulerAngles.y, rotation.z);
                        break;
                }
            }

            if (shape && shape.RawPosition.Horizontal().Same(position.Value.Horizontal()) && shape.RawRotation.eulerAngles.Same(rotation))
            {
                yield return new WaitForSeconds(GetPermaDropDelay());
                shape.permaDrop = true;
            }
        }

        public override (float, float) GetMovement()
        {
            return (0, 0);
        }

        public override bool IsControllingShape(Shape shape, bool notInMenu = false)
        {
            return true;
        }

        public override bool ShouldRotateShape()
        {
            return true;
        }

        public override bool ShouldSendPacket()
        {
            return base.ShouldSendPacket() && networkController.Server?.Running == true;
        }

        protected override void OnControllingShapeCreated(Shape shape)
        {
            if (!shape || !ShouldSendPacket() && !(this is DemoContainer))
                return;

            shape.StartCoroutine(GoToDestination(shape));
        }

        protected virtual float GetScanDelay()
        {
            return 0.5f;
        }

        protected virtual float GetMinimumMoveDelay()
        {
            return 0.25f;
        }

        protected virtual float GetMaximumMoveDelay()
        {
            return 0.75f;
        }

        protected virtual float GetPermaDropDelay()
        {
            return 0.5f;
        }
    }
}