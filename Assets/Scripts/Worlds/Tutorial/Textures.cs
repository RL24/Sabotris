using System.Collections.Generic;
using UnityEngine;

namespace Sabotris.Worlds.Tutorial
{
    public class Textures : MonoBehaviour
    {
        public Sprite
            circle,
            cross,
            square,
            triangle,
            dpadLeft, dpadUp, dpadRight, dpadDown,
            joystickBase,
            joystickThumb,
            l1, l2, l3,
            r1, r2, r3,
            keyboardBase,
            keyU, keyI, keyO, keyJ, keyK, keyL;

        private readonly Dictionary<string, Sprite> _textures = new Dictionary<string, Sprite>();

        private void Start()
        {
            _textures.Add("<Gamepad>/buttonEast", circle);
            _textures.Add("<Gamepad>/buttonSouth", cross);
            _textures.Add("<Gamepad>/buttonWest", square);
            _textures.Add("<Gamepad>/buttonNorth", triangle);
            _textures.Add("<Gamepad>/dpad/left", dpadLeft);
            _textures.Add("<Gamepad>/dpad/up", dpadUp);
            _textures.Add("<Gamepad>/dpad/right", dpadRight);
            _textures.Add("<Gamepad>/dpad/down", dpadDown);
            _textures.Add("<Gamepad>/leftShoulder", l1);
            _textures.Add("<Gamepad>/leftTrigger", l2);
            _textures.Add("<Gamepad>/leftStickPress", l3);
            _textures.Add("<Gamepad>/rightShoulder", r1);
            _textures.Add("<Gamepad>/rightTrigger", r2);
            _textures.Add("<Gamepad>/rightStickPress", r3);
            
            _textures.Add("<Keyboard>/u", keyU);
            _textures.Add("<Keyboard>/i", keyI);
            _textures.Add("<Keyboard>/o", keyO);
            _textures.Add("<Keyboard>/j", keyJ);
            _textures.Add("<Keyboard>/k", keyK);
            _textures.Add("<Keyboard>/l", keyL);
        }

        public Sprite GetMapped(string path) => _textures.TryGetValue(path, out var sprite) ? sprite : keyboardBase;
    }
}