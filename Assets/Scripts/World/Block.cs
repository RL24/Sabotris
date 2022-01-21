using System;
using System.Collections;
using Sabotris.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sabotris
{
    public class Block : MonoBehaviour
    {
        private Container _parentContainer;

        public Guid id;

        public bool doRemove;
        [SerializeField] private Vector3Int rawPosition = ShapeUtil.NullVector3Int;

        private void Start()
        {
            _parentContainer = GetComponentInParent<Container>();
        }

        private void Update()
        {
        }
        
        private void FixedUpdate()
        {
            if (rawPosition != ShapeUtil.NullVector3Int)
                transform.position = Vector3.Lerp(transform.position, _parentContainer.transform.position + RawPosition, GameSettings.TransitionSpeed);

            if (!doRemove)
                return;
            
            transform.localScale = Vector3.Lerp(transform.localScale, doRemove ? Vector3.zero : Vector3.one, GameSettings.TransitionSpeed);
            if (transform.localScale.GetMinValue() < 0.01)
                Destroy(gameObject);
        }

        public IEnumerator Remove(int index = -1, int max = -1)
        {
            var delay = index == -1 ? Random.Range(0, 0.5f) : index * (5f / max);
            yield return new WaitForSeconds(delay);
            doRemove = true;
        }

        public Vector3Int RawPosition
        {
            get => rawPosition == ShapeUtil.NullVector3Int 
                ? Vector3Int.RoundToInt(transform.position - _parentContainer.transform.position)
                : rawPosition;
            set
            {
                if (rawPosition == value)
                    return;
                _parentContainer = GetComponentInParent<Container>();
                rawPosition = value;
            }
        }
        
    }
}
