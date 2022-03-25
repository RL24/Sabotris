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
        public Container parentContainer;
        public Shape parentShape;
        public Guid id;
        public Color? color;

        public bool shifted;
        public bool doRemove;
        [SerializeField] private Vector3Int rawPosition = ShapeUtil.NullVector3Int;

        private int _index;
        private float _poweredBubbleSpeed;
        private float _poweredBubbleAmount;
        
        private void Start()
        {
            _index = Random.Range(0, 100);
            _poweredBubbleSpeed = Random.Range(2f, 4f);
            _poweredBubbleAmount = Random.Range(0.05f, 0.15f);
            
            if (color != null)
            {
                foreach (var ren in GetComponentsInChildren<Renderer>())
                    ren.material.color = color ?? Color.white;
            }

            if (rawPosition != ShapeUtil.NullVector3Int)
                transform.localScale = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (rawPosition != ShapeUtil.NullVector3Int && shifted)
                transform.position = Vector3.Lerp(transform.position, parentContainer.transform.position + rawPosition, GameSettings.Settings.gameTransitionSpeed);

            var bubble = parentShape && parentShape.PowerUp != null && !parentShape.locked;
            var targetScale = Vector3.one * (!doRemove).Int();
            if (bubble && !doRemove)
                targetScale += Vector3.one * (Mathf.Sin((Time.time + _index) * _poweredBubbleSpeed) * _poweredBubbleAmount + _poweredBubbleAmount);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, GameSettings.Settings.gameTransitionSpeed);

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
                ? Vector3Int.RoundToInt(transform.position - parentContainer.transform.position)
                : rawPosition;
            set
            {
                if (rawPosition == value)
                    return;
                parentContainer = GetComponentInParent<Container>();
                rawPosition = value;
            }
        }
    }
}