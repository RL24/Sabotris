using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sabotris.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sabotris
{
    public class DemoContainer : Container
    {
        private readonly Quaternion[] _rotations = {
            Quaternion.Euler(0, 0, 0),
            Quaternion.Euler(90, 0, 0),
            Quaternion.Euler(180, 0, 0),
            Quaternion.Euler(270, 0, 0),
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, 270, 0),
            Quaternion.Euler(0, 0, 90),
            Quaternion.Euler(0, 90, 90),
            Quaternion.Euler(0, 180, 90),
            Quaternion.Euler(0, 270, 90),
            Quaternion.Euler(90, 90, 0),
            Quaternion.Euler(270, 90, 0)
        };
        
        private (Vector3Int?, Quaternion) _destination;

        private List<(Vector3Int[], Quaternion)> _offsets = new List<(Vector3Int[], Quaternion)>();

        protected override void Start()
        {
            base.Start();

            OnEnable();
        }
        
        protected void OnEnable()
        {
            if (!ControllingShape)
                StartDropping();
        }

        private IEnumerable<Vector3Int> GetEmptySpaces()
        {
            var spaces = new List<Vector3Int>();
            for (var x = -Radius; x <= Radius; x++)
            {
                for (var z = -Radius; z <= Radius; z++)
                {
                    for (var y = DropPosition.y; y > 0; y--)
                    {
                        var vec = new Vector3Int(x, y - 1, z);
                        if (DoesCollide(new[] {vec}))
                        {
                            spaces.Add(new Vector3Int(x, y, z));
                            break;
                        }
                    }
                }
            }
            return spaces;
        }

        private (Vector3Int?, Quaternion) GetNextSpaceAndRotation()
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

            // var (bestSpace, bestRotation, _) = availableSpaces.OrderBy((space) => space.Item1.y)
            //     .Where((space) => space.Item1.y == availableSpaces.First().Item1.y)
            //     .OrderBy((space) => space.Item3.Sum((offset => offset.y * 100)))
            //     .First();
            
            availableSpaces = availableSpaces.OrderBy((space) => space.Item1.y).ToList();
            var lowestLayer = availableSpaces.First().Item1.y;
            var lowestGroup = availableSpaces.Where((space) => space.Item1.y == lowestLayer).ToList();
            var best = lowestGroup.OrderBy((space) => space.Item3.Sum((offset) => offset.y * 100)).ToList();
            var (bestSpace, bestRotation, _) = best.First();
            
            return (bestSpace, bestRotation);
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
            yield return new WaitForSeconds(1);
            
            _offsets = new List<(Vector3Int[], Quaternion)>();
            var prevRotation = shape.RawRotation;
            foreach (var rot in _rotations)
            {
                shape.RawRotation = rot;
                _offsets.Add((shape.Offsets.Select((offset) => offset.Item2.Copy()).ToArray(), rot));
            }
            shape.RawRotation = prevRotation;

            var next = GetNextSpaceAndRotation();
            _destination = next;
            
            var position = _destination.Item1;
            var rotation = _destination.Item2.eulerAngles;
            
            if (position == null)
                yield break;

            while (shape.RawPosition.x != position.Value.x
                   || shape.RawPosition.z != position.Value.z
                   || !shape.RawRotation.eulerAngles.x.Same(rotation.x)
                   || !shape.RawRotation.eulerAngles.y.Same(rotation.y)
                   || !shape.RawRotation.eulerAngles.z.Same(rotation.z))
            {
                yield return new WaitForSeconds(Random.Range(0.25f, 0.75f));

                if (!shape)
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
         
                switch (choices[Random.Range(0, choices.Count)])
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
                shape.permaDrop = true;
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
        
        protected override void OnControllingShapeCreated(Shape shape)
        {
            if (!shape)
                return;
            
            shape.StartCoroutine(GoToDestination(shape));
        }
        
        protected override int GetDropSpeed()
        {
            return 1000;
        }
    }
}