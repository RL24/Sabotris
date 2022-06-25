using Sabotris.IO;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Worlds.Tutorial
{
    public class TutorialItem : MonoBehaviour
    {
        public SpriteRenderer left, right;

        public Vector3 lerpPosition;
        public Quaternion lerpRotation, itemRotation;

        private void FixedUpdate()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, lerpPosition, GameSettings.Settings.gameTransitionSpeed.FixedDelta());
            transform.localRotation = Quaternion.Lerp(transform.localRotation, lerpRotation, GameSettings.Settings.gameTransitionSpeed.FixedDelta());
            
            left.transform.rotation = itemRotation;
            right.transform.rotation = itemRotation;
        }
    }
}