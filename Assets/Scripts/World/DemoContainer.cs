using System;
using System.Linq;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris
{
    public class DemoContainer : Container
    {
        private readonly Vector3Int[][] _demoShapes =
        {
            new[]
            {
                new Vector3Int(-1, 0, 2),
                new Vector3Int(-2, 0, 2),
                new Vector3Int(-2, 0, 1),
                new Vector3Int(-2, 0, 0),
            },

            new[]
            {
                new Vector3Int(-2, 0, -1),
                new Vector3Int(-1, 0, -1),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 0, 0),
            },

            new[]
            {
                new Vector3Int(-1, 0, 1),
                new Vector3Int(0, 0, 1),
                new Vector3Int(1, 0, 1),
                new Vector3Int(0, 0, 2),
            },

            new[]
            {
                new Vector3Int(1, 0, -1),
                new Vector3Int(2, 0, -1),
                new Vector3Int(1, 0, 0),
                new Vector3Int(2, 0, 0),
            },

            new[]
            {
                new Vector3Int(-2, 0, -2),
                new Vector3Int(-1, 0, -2),
                new Vector3Int(0, 0, -2),
                new Vector3Int(1, 0, -2),
            },

            new[]
            {
                new Vector3Int(0, 1, -2),
                new Vector3Int(0, 1, -1),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, 0, -1),
            },

            new[]
            {
                new Vector3Int(2, 0, 1),
                new Vector3Int(2, 0, 2),
                new Vector3Int(2, 1, 1),
                new Vector3Int(2, 2, 1),
            },

            new[]
            {
                new Vector3Int(2, 0, -2),
                new Vector3Int(2, 1, -2),
                new Vector3Int(2, 2, -2),
                new Vector3Int(2, 1, -1),
            },

            new[]
            {
                new Vector3Int(-1, 1, 2),
                new Vector3Int(0, 1, 2),
                new Vector3Int(1, 1, 2),
                new Vector3Int(1, 0, 2),
            },

            new[]
            {
                new Vector3Int(-2, 0, -2),
                new Vector3Int(-2, 0, -1),
                new Vector3Int(-1, 0, -2),
                new Vector3Int(-1, 0, -1),
            },

            new[]
            {
                new Vector3Int(-1, 0, 0),
                new Vector3Int(-1, 0, 1),
                new Vector3Int(0, 0, 1),
                new Vector3Int(1, 0, 1),
            },

            new[]
            {
                new Vector3Int(-2, 0, 0),
                new Vector3Int(-2, 0, 1),
                new Vector3Int(-2, 1, 1),
                new Vector3Int(-2, 0, 2),
            },

            new[]
            {
                new Vector3Int(2, 0, 2),
                new Vector3Int(2, 1, 2),
                new Vector3Int(2, 2, 2),
                new Vector3Int(2, 3, 2),
            },

            new[]
            {
                new Vector3Int(0, 1, 0),
                new Vector3Int(-1, 1, 0),
                new Vector3Int(-1, 1, 1),
                new Vector3Int(0, 1, 1),
            },

            new[]
            {
                new Vector3Int(1, 0, -1),
                new Vector3Int(1, 0, 0),
                new Vector3Int(1, 1, 0),
                new Vector3Int(1, 1, 1),
            },

            new[]
            {
                new Vector3Int(0, 1, -2),
                new Vector3Int(1, 1, -2),
                new Vector3Int(1, 0, -2),
                new Vector3Int(1, 2, -2),
            },

            new[]
            {
                new Vector3Int(2, 0, 0),
                new Vector3Int(2, 1, 0),
                new Vector3Int(2, 1, -1),
                new Vector3Int(2, 2, -1),
            },

            new[]
            {
                new Vector3Int(-2, 0, 2),
                new Vector3Int(-1, 0, 2),
                new Vector3Int(0, 0, 2),
                new Vector3Int(1, 0, 2),
            },

            new[]
            {
                new Vector3Int(-1, 0, -2),
                new Vector3Int(-1, 0, -1),
                new Vector3Int(0, 0, -1),
                new Vector3Int(1, 0, -1),
            },

            new[]
            {
                new Vector3Int(-2, 0, -2),
                new Vector3Int(-2, 0, -1),
                new Vector3Int(-2, 0, 0),
                new Vector3Int(-2, 1, -1),
            },

            new[]
            {
                new Vector3Int(0, 0, 2),
                new Vector3Int(1, 0, 2),
                new Vector3Int(0, 1, 2),
                new Vector3Int(1, 1, 2),
            },

            new[]
            {
                new Vector3Int(-2, 0, 0),
                new Vector3Int(-2, 0, 1),
                new Vector3Int(-2, 0, 2),
                new Vector3Int(-1, 0, 2),
            },

            new[]
            {
                new Vector3Int(-1, 0, 1),
                new Vector3Int(0, 0, 1),
                new Vector3Int(1, 0, 1),
                new Vector3Int(2, 0, 1),
            },

            new[]
            {
                new Vector3Int(0, 0, -1),
                new Vector3Int(1, 0, -1),
                new Vector3Int(1, 0, 0),
                new Vector3Int(2, 0, 0),
            },

            new[]
            {
                new Vector3Int(-2, 0, -2),
                new Vector3Int(-1, 0, -2),
                new Vector3Int(0, 0, -2),
                new Vector3Int(0, 1, -2),
            },

            new[]
            {
                new Vector3Int(-2, 1, 0),
                new Vector3Int(-1, 1, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 0, 0),
            },

            new[]
            {
                new Vector3Int(2, 0, -2),
                new Vector3Int(2, 1, -2),
                new Vector3Int(2, 1, -1),
                new Vector3Int(2, 1, 0),
            },

            new[]
            {
                new Vector3Int(-2, 1, -1),
                new Vector3Int(-1, 1, -1),
                new Vector3Int(0, 1, -1),
                new Vector3Int(-1, 0, -1),
            },

            new[]
            {
                new Vector3Int(-2, 0, 2),
                new Vector3Int(-2, 0, 1),
                new Vector3Int(-1, 0, 2),
                new Vector3Int(-1, 0, 1),
                new Vector3Int(0, 0, 1),
            },

            new[]
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(1, 0, -1),
                new Vector3Int(1, 0, 1),
                new Vector3Int(2, 0, 1),
            },

            new[]
            {
                new Vector3Int(-2, 0, -2),
                new Vector3Int(-1, 0, -2),
                new Vector3Int(1, 0, -2),
            }
        };

        private int _index;

        protected override void Start()
        {
            DropPosition = new Vector3Int(0, 7, 0);

            base.Start();

            StartDropping(GetNextOffsets());
        }

        public Pair<Guid, Vector3Int>[] GetNextOffsets()
        {
            return _demoShapes[_index++ % _demoShapes.Length]
                .Select((offset) => new Pair<Guid, Vector3Int>(Guid.NewGuid(), offset)).ToArray();
        }
    }
}