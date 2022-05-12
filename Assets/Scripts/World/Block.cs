using System;
using System.Collections;
using Sabotris.IO;
using Sabotris.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sabotris
{
    public class Block : MonoBehaviour
    {
        private Container _parentContainer;

        public Guid Id;
        public Color? BlockColor;

        public bool shifted;
        public bool doRemove;
        [SerializeField] private Vector3Int rawPosition = ShapeUtil.NullVector3Int;

        private void Start()
        {
            _parentContainer = GetComponentInParent<Container>();

            if (BlockColor != null)
            {
                foreach (var ren in GetComponentsInChildren<Renderer>())
                    ren.material.color = BlockColor ?? Color.white;
            }

            if (rawPosition != ShapeUtil.NullVector3Int)
                transform.localScale = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (rawPosition != ShapeUtil.NullVector3Int && shifted)
                transform.position = Vector3.Lerp(transform.position, _parentContainer.transform.position + rawPosition, GameSettings.Settings.gameTransitionSpeed.FixedDelta());

            transform.localScale = Vector3.Lerp(transform.localScale, doRemove ? Vector3.zero : Vector3.one, GameSettings.Settings.gameTransitionSpeed.FixedDelta());

            if (doRemove && transform.localScale.GetMinValue() < 0.01)
                Destroy(gameObject);
        }

        public IEnumerator Remove(int index = -1, int max = -1)
        {
            var delay = index == -1 ? Random.Range(0, 0.4f) : index * (3f / max);
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