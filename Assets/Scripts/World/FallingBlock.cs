using System;
using Sabotris.IO;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris
{
    public class FallingBlock : MonoBehaviour
    {
        public Container parentContainer;
        public Guid id;
        public Color? color;
        public Vector3 position;
        public bool removed;

        private Vector3Int _startPosition;
        private Vector3Int _targetPosition;
        private float _velocity;
        
        private void Start()
        {
            if (color != null)
                foreach (var ren in GetComponentsInChildren<Renderer>())
                    ren.material.color = color ?? Color.white;

            transform.localScale = Vector3.zero;

            _startPosition = position.Round(1);
            _targetPosition = parentContainer.GetDropToPosition(_startPosition);
            
            Debug.Log(_targetPosition);
        }

        private void Update()
        {
            _targetPosition = parentContainer.GetDropToPosition(_startPosition);

            if (!removed)
            {
                _velocity += 0.0005f.Delta();
                position += Vector3.down * _velocity;
            }

            if (position.y <= _targetPosition.y)
            {
                position = _targetPosition;
                _velocity = 0;
                removed = true;
            }
            
            transform.position = Vector3.Lerp(transform.position, position, GameSettings.Settings.gameTransitionSpeed.Delta());
        }

        private void FixedUpdate()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, GameSettings.Settings.gameTransitionSpeed.FixedDelta());
        }
    }
}