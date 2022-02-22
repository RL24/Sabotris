using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sabotris.Util
{
    public static class ShapeUtil
    {
        public static readonly Vector3Int NullVector3Int = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        private static readonly Vector3Int[] HorizontalDirections = {Vector3Int.left, Vector3Int.forward, Vector3Int.right, Vector3Int.back};
        private static readonly Vector3Int[] OmniDirections = {Vector3Int.left, Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.up, Vector3Int.down};

        private static readonly List<Vector3Int[]> GeneratedShapes = new List<Vector3Int[]>();

        public static (Guid, Vector3Int)[] Generate(int offsetCount, bool vertical, Vector3Int bottomLeft, Vector3Int topRight, int regenerated = 0)
        {
            for (;;)
            {
                regenerated++;
                var offsets = new List<Vector3Int> {Vector3Int.zero};
                while (offsets.Count < offsetCount)
                {
                    var free = new List<Vector3Int>();
                    foreach (var adjacent in from offset in offsets let directionals = vertical ? OmniDirections : HorizontalDirections from direction in directionals select offset + direction into adjacent where !offsets.Contains(adjacent) && !free.Contains(adjacent) && (bottomLeft == NullVector3Int || topRight == NullVector3Int || !adjacent.IsOutside(bottomLeft, topRight)) select adjacent)
                        free.Add(adjacent);

                    if (!free.Any()) break;

                    var pick = Random.Range(0, free.Count);
                    offsets.Add(free[pick]);
                }

                var centerOffset = Vector3Int.RoundToInt(new Vector3((float) offsets.Average((offset) => offset.x), (float) offsets.Average((offset) => offset.y), (float) offsets.Average((offset) => offset.z)));

                var centered = offsets.Select((offset) => offset - centerOffset).ToArray();
                var generated = centered.Select((offset) => (Guid.NewGuid(), offset)).ToArray();
                if (regenerated < 10 && GeneratedShapes.Any(alreadyGenerated => alreadyGenerated.Same(centered)))
                    continue;
                if (regenerated >= 10)
                    GeneratedShapes.Clear();

                GeneratedShapes.Add(centered);
                return generated;
            }
        }
    }
}