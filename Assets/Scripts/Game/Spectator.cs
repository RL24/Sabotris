using System;
using Sabotris.IO;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Game
{
    public class Spectator : MonoBehaviour
    {
        public Vector3 position;
        public Quaternion rotation;
        
        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, position, GameSettings.Settings.gameTransitionSpeed.FixedDelta());
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, GameSettings.Settings.gameTransitionSpeed.FixedDelta());
        }
    }
}